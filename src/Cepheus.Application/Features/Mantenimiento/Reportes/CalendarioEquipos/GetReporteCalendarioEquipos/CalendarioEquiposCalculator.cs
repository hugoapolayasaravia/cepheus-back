using Cepheus.Domain.Mantenimiento.Enum;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioEquipos.GetReporteCalendarioEquipos;

internal sealed record PlanCalendario(
    string PlantaCode,
    string PlanCode,
    string EquipoCode,
    string ActividadCode,
    string Gama,
    decimal Frecuencia,
    UnidadFrecuencia Unidad,
    decimal FrecuenciaGama,
    decimal TiempoRestante,
    string SubCentroCostoCode);

internal sealed record GamaCalculada(PlanCalendario Plan, int? Prioridad, IReadOnlyList<CeldaCalculada> Celdas);

internal sealed record CeldaCalculada(DateTime Fecha, decimal Dato, bool HayOrden);

/// <summary>
/// Traducción de la lógica del cursor de Man_sp_Listado_Calendario_Equipos a un cálculo en memoria (sin tablas
/// temporales ni SQL dinámico). Función pura: todo lo que necesita entra por parámetros.
///
/// Reglas (idénticas al SP):
///  - Los domingos no cuentan: ni se calculan ni descuentan.
///  - Cada día hábil el "dato" baja en Frecuencia (la unidad Día/Semana/Mes/Hora es solo informativa).
///  - Si la unidad (subcentro de costo) tuvo parada ese día, no descuenta.
///  - Si hay una OT del plan ese día, el ciclo se reinicia a FrecuenciaGama y el dato termina en .05 ese día.
///  - Marcas decimales: dato = 0 -> toca; si el dato salta de positivo a negativo sin pasar por 0 se marca -0.01
///    (primer día vencido); después los vencidos vuelven a enteros.
///  - Si la actividad tiene prioridad (existe en ActividadMantenimiento), una OT suya reinicia el ciclo de las
///    actividades que cubre, del mismo equipo, desde ese día en adelante.
///  - Las actividades de un equipo se procesan por prioridad ascendente, para que las cubiertas ya existan
///    cuando la que las cubre las recalcula.
///
/// Diferencias deliberadas frente al SP:
///  - La bandera de negativos del recálculo del SP se arrastraba entre equipos (nunca se reiniciaba). Aquí se
///    reinicia en cada pasada de recálculo.
///  - Si la actividad cubierta no tiene plan en ese equipo, se omite (el SP reutilizaba valores de la
///    iteración anterior).
///  - Las OT se asocian al plan por (planta, plan) y no solo por código de plan.
///  - Si un equipo tiene dos planes con la misma actividad, solo se calcula el primero (el SP también
///    terminaba calculando solo uno).
/// </summary>
internal static class CalendarioEquiposCalculator
{
    private const decimal MarcaOrden = 0.05m;
    private const decimal MarcaVencido = 0.01m;

    public static IReadOnlyList<DateTime> DiasHabiles(DateTime desde, DateTime hasta)
    {
        var dias = new List<DateTime>();
        for (var d = desde.Date; d <= hasta.Date; d = d.AddDays(1))
        {
            if (d.DayOfWeek != DayOfWeek.Sunday) dias.Add(d);
        }
        return dias;
    }

    public static IReadOnlyList<GamaCalculada> Calcular(
        DateTime desde,
        DateTime hasta,
        IReadOnlyList<PlanCalendario> planes,
        IReadOnlyDictionary<string, int> prioridades,
        ILookup<string, string> cubiertas,
        IReadOnlySet<(string Planta, string Plan, DateTime Fecha)> ordenes,
        IReadOnlySet<(string SubCentroCosto, DateTime Fecha)> paradas)
    {
        var dias = DiasHabiles(desde, hasta);

        int? PrioridadDe(string actividad) => prioridades.TryGetValue(actividad, out var p) ? p : null;

        // Orden del cursor legacy: equipo, prioridad (NULL primero). Desempate estable.
        var ordenados = planes
            .OrderBy(p => p.EquipoCode, StringComparer.Ordinal)
            .ThenBy(p => PrioridadDe(p.ActividadCode) ?? int.MinValue)
            .ThenBy(p => p.ActividadCode, StringComparer.Ordinal)
            .ThenBy(p => p.PlantaCode, StringComparer.Ordinal)
            .ThenBy(p => p.PlanCode, StringComparer.Ordinal)
            .ToList();

        var procesados = new List<PlanCalendario>();
        var planPorEquipoActividad = new Dictionary<(string Equipo, string Actividad), PlanCalendario>();
        var datos = new Dictionary<(string Equipo, string Actividad), Dictionary<DateTime, decimal>>();

        foreach (var plan in ordenados)
        {
            var clave = (plan.EquipoCode, plan.ActividadCode);
            if (planPorEquipoActividad.ContainsKey(clave)) continue;

            planPorEquipoActividad[clave] = plan;
            procesados.Add(plan);
            datos[clave] = new Dictionary<DateTime, decimal>();

            var tienePrioridad = PrioridadDe(plan.ActividadCode).HasValue;
            var restante = plan.TiempoRestante;
            var banderaOrden = false;
            var banderaNegativo = false;

            foreach (var dia in dias)
            {
                restante -= plan.Frecuencia;

                if (paradas.Contains((plan.SubCentroCostoCode, dia)))
                {
                    restante += plan.Frecuencia;
                }

                var hayOrden = ordenes.Contains((plan.PlantaCode, plan.PlanCode, dia));

                if (banderaOrden)
                {
                    restante -= MarcaOrden;
                    banderaOrden = false;
                }

                if (hayOrden)
                {
                    restante = plan.FrecuenciaGama + MarcaOrden;
                    banderaOrden = true;
                }

                if (restante == 0m)
                {
                    banderaNegativo = true;
                }

                if (Fraccion(restante) == -MarcaVencido)
                {
                    restante += MarcaVencido;
                }

                if (restante < 0m && !banderaNegativo)
                {
                    restante -= MarcaVencido;
                    banderaNegativo = true;
                }

                if (tienePrioridad)
                {
                    if (hayOrden)
                    {
                        foreach (var cubierta in cubiertas[plan.ActividadCode])
                        {
                            RecalcularCubierta(plan.EquipoCode, cubierta, dia, hasta, dias,
                                planPorEquipoActividad, datos, ordenes);
                        }
                    }

                    if (Fraccion(restante) == -MarcaVencido)
                    {
                        foreach (var cubierta in cubiertas[plan.ActividadCode])
                        {
                            if (datos.TryGetValue((plan.EquipoCode, cubierta), out var celdas) &&
                                celdas.TryGetValue(dia, out var valor) &&
                                Fraccion(valor) == -MarcaVencido)
                            {
                                celdas[dia] = valor + MarcaVencido;
                            }
                        }
                    }
                }

                datos[clave][dia] = restante;
            }
        }

        return procesados
            .Select(p => new GamaCalculada(
                p,
                PrioridadDe(p.ActividadCode),
                dias.Select(d => new CeldaCalculada(
                    d,
                    datos[(p.EquipoCode, p.ActividadCode)][d],
                    ordenes.Contains((p.PlantaCode, p.PlanCode, d)))).ToList()))
            .OrderBy(g => g.Prioridad ?? int.MinValue)
            .ThenBy(g => g.Plan.EquipoCode, StringComparer.Ordinal)
            .ThenBy(g => g.Plan.ActividadCode, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Una OT de la actividad que cubre reinicia el ciclo de la cubierta desde ese día: el dato parte de su
    /// FrecuenciaGama y baja por su Frecuencia. Solo actualiza días que la cubierta ya tiene calculados.
    /// </summary>
    private static void RecalcularCubierta(
        string equipo,
        string actividadCubierta,
        DateTime desdeDia,
        DateTime hasta,
        IReadOnlyList<DateTime> dias,
        IReadOnlyDictionary<(string Equipo, string Actividad), PlanCalendario> planPorEquipoActividad,
        IReadOnlyDictionary<(string Equipo, string Actividad), Dictionary<DateTime, decimal>> datos,
        IReadOnlySet<(string Planta, string Plan, DateTime Fecha)> ordenes)
    {
        if (!planPorEquipoActividad.TryGetValue((equipo, actividadCubierta), out var planCubierta)) return;
        if (!datos.TryGetValue((equipo, actividadCubierta), out var celdas)) return;

        var valor = planCubierta.FrecuenciaGama;
        var banderaOrden = false;
        var banderaNegativo = false;

        foreach (var dia in dias.Where(d => d >= desdeDia))
        {
            var hayOrden = ordenes.Contains((planCubierta.PlantaCode, planCubierta.PlanCode, dia));

            if (banderaOrden)
            {
                valor -= MarcaOrden;
                banderaOrden = false;
            }

            if (hayOrden)
            {
                valor += MarcaOrden;
                banderaOrden = true;
            }

            if (valor == 0m)
            {
                banderaNegativo = true;
            }

            if (Fraccion(valor) == -MarcaVencido)
            {
                valor += MarcaVencido;
            }

            if (valor < 0m && !banderaNegativo)
            {
                valor -= MarcaVencido;
                banderaNegativo = true;
            }

            if (celdas.ContainsKey(dia))
            {
                celdas[dia] = valor;
            }

            valor -= planCubierta.Frecuencia;
        }
    }

    /// <summary>Parte decimal con truncamiento hacia cero (equivale a x - CONVERT(integer, x) de T-SQL).</summary>
    private static decimal Fraccion(decimal valor) => valor - decimal.Truncate(valor);

    /// <summary>
    /// Criterio de "Genera OT" del legacy: dato = 0, o primer día vencido (parte decimal .99 con el Int() de
    /// PowerBuilder, que redondea hacia abajo: -2.01 -> .99).
    /// </summary>
    public static bool TocaGenerarOrden(decimal dato)
        => dato == 0m || Math.Round(dato - Math.Floor(dato), 2) == 0.99m;
}
