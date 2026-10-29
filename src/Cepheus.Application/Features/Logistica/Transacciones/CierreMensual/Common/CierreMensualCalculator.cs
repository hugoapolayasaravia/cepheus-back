using Cepheus.Application.Features.Logistica.Transacciones.Kardex.Common;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;

/// <summary>Nota de Crédito por ajuste de precio (motivo 01) del período: no mueve stock.</summary>
public sealed record AjustePrecioNota(string DocumentoCode, DateTime Fecha, decimal Cantidad, decimal CostoUnitario);

public sealed record CierreArticuloEntrada(
    string ArticuloCode,
    decimal BaseCantidad,
    decimal BaseCosto,
    IReadOnlyList<KardexEntrada> Movimientos,
    IReadOnlyList<AjustePrecioNota> NotasAjuste);

public sealed record IncidenciaCalculada(TipoIncidenciaCierre Tipo, string? DocumentoCode, string Mensaje);

/// <summary>Salida cuyo documento debe quedar valorizado a PrecioNuevo.</summary>
public sealed record SalidaARevaluar(long MovimientoId, OrigenMovimientoArticulo Origen, decimal Cantidad, decimal PrecioNuevo);

public sealed record CierreArticuloResultado(
    string ArticuloCode,
    decimal Cantidad,
    decimal CostoPromedio,
    decimal CantidadIngresos,
    decimal CantidadSalidas,
    /// <summary>Promedio del mes cuando hubo Nota de Crédito de ajuste (todas las salidas del mes se valorizan con él).</summary>
    decimal? PromedioMesAjuste,
    IReadOnlyList<KardexLinea> Lineas,
    IReadOnlyList<SalidaARevaluar> Salidas,
    IReadOnlyList<IncidenciaCalculada> Incidencias)
{
    public bool TuvoMovimientos => Lineas.Count > 0;
}

/// <summary>
/// Cálculo del cierre de UN artículo en un período. Lógica pura, sin base de datos.
/// Combina Logi_sp_Actualiza_Mes_Consumos (valorización cronológica de las salidas) con
/// Logi_sp_Actualiza_Mes_Consumos_nc (promedio del mes cuando hay Nota de Crédito de ajuste, motivo 01).
///
/// Promedio del mes (legacy _nc):
///   Acumulado = saldo inicial + Σ cantidades de ingresos (compra, importación, orden de servicio)
///   Total     = saldo inicial × costo inicial + Σ cantidad × costo de esos ingresos
///               − Σ cantidad × costo de TODAS las notas de crédito (ajuste y devolución; la cantidad no se resta)
///   Promedio  = ROUND(Total / Acumulado, 6)
/// Si hay ajuste, ese promedio fija el costo desde la fecha de la nota y valoriza TODAS las salidas del mes.
/// En el primer cierre de una planta, la Apertura cuenta como saldo inicial.
/// </summary>
public static class CierreMensualCalculator
{
    private static readonly OrigenMovimientoArticulo[] OrigenesQueCompran =
    {
        OrigenMovimientoArticulo.NotaIngreso,
        OrigenMovimientoArticulo.Importacion,
        OrigenMovimientoArticulo.OrdenServicio,
        OrigenMovimientoArticulo.TransferenciaIngreso
    };

    private static readonly OrigenMovimientoArticulo[] OrigenesRevaluables =
    {
        OrigenMovimientoArticulo.ValeSalida,
        OrigenMovimientoArticulo.AjusteSalida,
        OrigenMovimientoArticulo.OrdenServicio
    };

    public static CierreArticuloResultado Calcular(CierreArticuloEntrada entrada)
    {
        var anulados = KardexValorizador.IdsAnulados(entrada.Movimientos);
        var vigentes = entrada.Movimientos.Where(m => !anulados.Contains(m.Id)).ToList();
        var incidencias = new List<IncidenciaCalculada>();

        // --- promedio del mes (solo si hay notas de ajuste)
        decimal? promedioMes = null;
        var movimientos = entrada.Movimientos.ToList();

        if (entrada.NotasAjuste.Count > 0)
        {
            var apertura = vigentes.Where(m => m.Origen == OrigenMovimientoArticulo.Apertura).ToList();
            var compras = vigentes
                .Where(m => m.Tipo == TipoMovimientoArticulo.Ingreso && OrigenesQueCompran.Contains(m.Origen))
                .ToList();
            var devoluciones = vigentes.Where(m => m.Origen == OrigenMovimientoArticulo.NotaCredito).ToList();

            var acumulado = entrada.BaseCantidad
                            + apertura.Sum(m => m.Cantidad)
                            + compras.Sum(m => m.Cantidad);

            var total = (entrada.BaseCantidad * entrada.BaseCosto)
                        + apertura.Sum(m => m.Cantidad * m.CostoUnitario)
                        + compras.Sum(m => m.Cantidad * m.CostoUnitario)
                        - devoluciones.Sum(m => m.Cantidad * m.CostoUnitario)
                        - entrada.NotasAjuste.Sum(n => n.Cantidad * n.CostoUnitario);

            if (acumulado == 0)
            {
                var doc = entrada.NotasAjuste[0].DocumentoCode;
                incidencias.Add(new IncidenciaCalculada(
                    TipoIncidenciaCierre.AjustePrecioSinIngresos, doc,
                    $"La Nota de Crédito de ajuste {doc} no se pudo aplicar: el artículo no tuvo ingresos en el período."));
            }
            else
            {
                promedioMes = Math.Round(total / acumulado, KardexValorizador.DecimalesCosto, MidpointRounding.AwayFromZero);

                // Eventos de valorización sin movimiento de stock, ordenados por su fecha junto a los demás.
                long id = -1;
                foreach (var nota in entrada.NotasAjuste.OrderBy(n => n.Fecha).ThenBy(n => n.DocumentoCode))
                {
                    movimientos.Add(new KardexEntrada(
                        id--, nota.Fecha, nota.Fecha.ToString("yyyyMM"),
                        TipoMovimientoArticulo.Ingreso, OrigenMovimientoArticulo.AjustePrecio,
                        0m, promedioMes.Value, false, null));
                }
            }
        }

        var lineas = KardexValorizador.Calcular(entrada.BaseCantidad, entrada.BaseCosto, movimientos);

        // --- estado final
        var ultima = lineas.Count > 0 ? lineas[^1] : null;
        var cantidad = ultima?.SaldoCantidad ?? entrada.BaseCantidad;
        var promedio = ultima?.CostoPromedio ?? entrada.BaseCosto;

        var reales = lineas.Where(l => !l.Anulado && l.Entrada.Origen != OrigenMovimientoArticulo.AjustePrecio).ToList();

        var ingresos = reales
            .Where(l => l.Entrada.Tipo == TipoMovimientoArticulo.Ingreso && l.Entrada.Origen != OrigenMovimientoArticulo.Apertura)
            .Sum(l => l.Entrada.Cantidad);
        var salidas = reales
            .Where(l => l.Entrada.Tipo == TipoMovimientoArticulo.Salida)
            .Sum(l => l.Entrada.Cantidad);

        // --- salidas a revalorizar en sus documentos
        var aRevaluar = reales
            .Where(l => l.Entrada.Tipo == TipoMovimientoArticulo.Salida
                        && OrigenesRevaluables.Contains(l.Entrada.Origen))
            .Select(l => new SalidaARevaluar(
                l.Entrada.Id, l.Entrada.Origen, l.Entrada.Cantidad, promedioMes ?? l.CostoValorizado))
            .ToList();

        // --- incidencias
        var primerNegativo = lineas.FirstOrDefault(l => !l.Anulado && l.StockNegativo);
        if (primerNegativo is not null)
        {
            incidencias.Add(new IncidenciaCalculada(
                TipoIncidenciaCierre.StockNegativo, null,
                $"El saldo queda negativo ({primerNegativo.SaldoCantidad:0.##}) desde el {primerNegativo.Entrada.Fecha:dd/MM/yyyy}."));
        }

        var sinCosto = aRevaluar.FirstOrDefault(s => s.PrecioNuevo == 0m);
        if (sinCosto is not null)
        {
            incidencias.Add(new IncidenciaCalculada(
                TipoIncidenciaCierre.SalidaSinCosto, null,
                "Hay salidas valorizadas a costo cero: el artículo no tenía costo promedio al momento de salir."));
        }

        return new CierreArticuloResultado(
            entrada.ArticuloCode,
            Math.Round(cantidad, 2, MidpointRounding.AwayFromZero),
            promedio,
            ingresos,
            salidas,
            promedioMes,
            lineas,
            aRevaluar,
            incidencias);
    }
}
