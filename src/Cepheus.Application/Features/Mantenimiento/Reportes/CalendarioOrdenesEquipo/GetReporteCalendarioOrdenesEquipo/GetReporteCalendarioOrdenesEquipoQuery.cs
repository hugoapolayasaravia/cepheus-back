using Cepheus.Domain.Mantenimiento.Enum;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioOrdenesEquipo.GetReporteCalendarioOrdenesEquipo;

/// <summary>
/// Calendario de Órdenes de Trabajo por equipo en un rango de fechas (legacy
/// Man_sp_Listado_Calendario_Ot_Equipos). Los filtros en null equivalen a "T" (todos).
/// </summary>
/// <param name="PlantaCode">Planta de la OT (legacy @plt).</param>
/// <param name="EquipoCode">Coincidencia parcial del código de equipo (legacy LIKE %@Equipo%).</param>
/// <param name="Estado">Estado de la OT (legacy @Estado contra Codigo_Est).</param>
public sealed record GetReporteCalendarioOrdenesEquipoQuery(
    string? PlantaCode,
    string? EquipoCode,
    EstadoOrdenTrabajo? Estado,
    DateTime FechaDesde,
    DateTime FechaHasta) : IRequest<ReporteCalendarioOrdenesEquipoResponse>;
