using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.ChangeOrdenTrabajoEstado
{
    public record ChangeOrdenTrabajoEstadoCommand(
        string PlantaCode,
        string Code,
        string NuevoEstado
    ) : IRequest<OrdenTrabajoResponse>;
}
