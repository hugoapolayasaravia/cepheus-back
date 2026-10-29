using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.HorometroCalendario.GetReporteHorometroCalendario;

/// <summary>
/// Calendario de lecturas de horómetro por unidad en un rango de fechas (legacy
/// Man_sp_Listado_Equipos_Horometro_Calendario). Los filtros en null equivalen a "T" (todos).
/// </summary>
/// <param name="CentroCostoCode">Centro de costo del subcentro (legacy @cc).</param>
/// <param name="PlantaCode">Planta del subcentro (legacy @plt).</param>
public sealed record GetReporteHorometroCalendarioQuery(
    string? CentroCostoCode,
    string? PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta) : IRequest<ReporteHorometroCalendarioResponse>;
