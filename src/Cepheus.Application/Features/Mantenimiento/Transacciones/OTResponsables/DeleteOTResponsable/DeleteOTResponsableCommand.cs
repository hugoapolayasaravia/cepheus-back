using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.DeleteOTResponsable
{
    public record DeleteOTResponsableCommand(
        string PlantaCode,
        string OrdenTrabajoCode,
        DateTime FechaProceso,
        string TrabajadorCode
    ) : IRequest;
}
