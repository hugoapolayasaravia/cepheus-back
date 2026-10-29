using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTRMateriales.DeleteOTRMaterial
{
    public record DeleteOTRMaterialCommand(
        string PlantaCode,
        string OrdenTrabajoCode,
        DateTime FechaProceso,
        string ArticuloCode
    ) : IRequest;
}
