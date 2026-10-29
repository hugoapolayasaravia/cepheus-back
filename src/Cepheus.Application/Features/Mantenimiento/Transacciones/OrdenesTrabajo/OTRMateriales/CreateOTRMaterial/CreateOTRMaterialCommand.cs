using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTRMateriales.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTRMateriales.CreateOTRMaterial
{
    public record CreateOTRMaterialCommand(
        string PlantaCode,
        string OrdenTrabajoCode,
        string ArticuloCode,
        DateTime? FechaProceso,
        decimal Cantidad,
        decimal CostoUnitario,
        decimal CostoTotal,
        string? EstadoCode
    ) : IRequest<OTRMaterialResponse>;
}
