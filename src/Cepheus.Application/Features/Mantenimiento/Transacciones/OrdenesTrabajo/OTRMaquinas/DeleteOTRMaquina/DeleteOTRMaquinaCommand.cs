using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTRMaquinas.DeleteOTRMaquina
{
    public record DeleteOTRMaquinaCommand(
        string PlantaCode,
        string OrdenTrabajoCode,
        string MaquinaCode
    ) : IRequest;
}
