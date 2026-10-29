using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioEquipos.GetReporteCalendarioEquipos;

/// <summary>
/// Calendario de Equipos: para cada plan de mantenimiento preventivo, el "dato" diario (tiempo restante hasta el
/// próximo preventivo) en un rango de fechas (legacy Man_sp_Listado_Calendario_Equipos). Los filtros en null
/// equivalen a "T" (todos).
/// </summary>
/// <param name="PlantaCode">Planta del plan (legacy @plt).</param>
/// <param name="EquipoCode">Coincidencia parcial del código de equipo (legacy LIKE %@Equipo%).</param>
public sealed record GetReporteCalendarioEquiposQuery(
    string? PlantaCode,
    string? EquipoCode,
    DateTime FechaDesde,
    DateTime FechaHasta) : IRequest<ReporteCalendarioEquiposResponse>;
