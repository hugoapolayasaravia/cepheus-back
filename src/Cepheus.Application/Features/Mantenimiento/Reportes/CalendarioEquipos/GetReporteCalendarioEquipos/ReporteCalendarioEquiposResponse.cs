using Cepheus.Domain.Mantenimiento.Enum;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioEquipos.GetReporteCalendarioEquipos;

public sealed record ReporteCalendarioEquiposResponse(
    DateTime FechaDesde,
    DateTime FechaHasta,
    // Días del rango SIN domingos (el legacy no cuenta ni calcula los domingos). Indice = columna "ite" del legacy.
    IReadOnlyList<ReporteCalendarioEquiposDiaResponse> Dias,
    int TotalGamas,
    IReadOnlyList<ReporteCalendarioEquiposGamaResponse> Gamas);

public sealed record ReporteCalendarioEquiposDiaResponse(int Indice, DateTime Fecha);

/// <summary>Una gama = una actividad preventiva de un equipo (un plan de mantenimiento preventivo).</summary>
public sealed record ReporteCalendarioEquiposGamaResponse(
    string PlantaCode,
    string PlanCode,
    string EquipoCode,
    string ActividadCode,
    // Verbo + objeto de la actividad (legacy Descripcion_Ver + ' ' + Descripcion_Sus).
    string Gama,
    decimal Frecuencia,
    UnidadFrecuencia Unidad,
    decimal FrecuenciaGama,
    decimal TiempoRestante,
    // null si la actividad no está en ActividadMantenimiento.
    int? Prioridad,
    IReadOnlyList<ReporteCalendarioEquiposCeldaResponse> Dias);

/// <param name="Dato">
/// Valor legacy: tiempo restante. 0 = toca hoy; -N.01 = primer día vencido; terminación .05 = hay OT ese día
/// (el ciclo se reinició).
/// </param>
/// <param name="HayOrden">Existe una OT del plan en ese día.</param>
/// <param name="TocaGenerarOrden">Dato = 0 o primer día vencido: el criterio con que "Genera OT" decide crear la OT.</param>
public sealed record ReporteCalendarioEquiposCeldaResponse(
    DateTime Fecha,
    decimal Dato,
    bool HayOrden,
    bool TocaGenerarOrden);
