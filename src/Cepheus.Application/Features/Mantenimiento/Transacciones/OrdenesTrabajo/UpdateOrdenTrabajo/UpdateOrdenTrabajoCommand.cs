using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.UpdateOrdenTrabajo
{
    /// <summary>
    /// Solo permite editar los campos "planificables" de la OT (no PlantaCode,
    /// Code ni Estado — el estado cambia por su propio endpoint dedicado).
    /// </summary>
    public record UpdateOrdenTrabajoCommand(
        string PlantaCode,
        string Code,
        string Description,
        DateTime FechaProceso,
        DateTime? FechaTermino,
        string ResponsableCode,
        string EspecialidadCode,
        string OportunidadCode,
        string EquipoCode,
        string PrioridadCode,
        string InspeccionCode,
        string TipoOrdenCode,
        string ActividadCode,
        string? SubCentroCostoCode,
        string? SubCentroEjecutorCode,
        string? PlanMantenimientoPreventivoCode,
        decimal DowntimeHours,
        decimal? Horometro,
        string? Observations,
        string Turno,
        byte[] RowVersion
    ) : IRequest<OrdenTrabajoResponse>;
}
