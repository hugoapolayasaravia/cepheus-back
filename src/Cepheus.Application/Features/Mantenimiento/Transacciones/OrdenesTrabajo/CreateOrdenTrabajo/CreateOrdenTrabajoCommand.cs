using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.CreateOrdenTrabajo
{
    /// <summary>
    /// No recibe Code (correlativo por planta, lo genera el backend) ni
    /// Estado (toda OT nace en EstadoOrdenTrabajo.Pendiente).
    /// </summary>
    public record CreateOrdenTrabajoCommand(
        string PlantaCode,
        string Description,
        DateTime FechaProceso,
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
        decimal? Horometro,
        string? Observations,
        int? UserId,
        string Turno
    ) : IRequest<OrdenTrabajoResponse>;
}
