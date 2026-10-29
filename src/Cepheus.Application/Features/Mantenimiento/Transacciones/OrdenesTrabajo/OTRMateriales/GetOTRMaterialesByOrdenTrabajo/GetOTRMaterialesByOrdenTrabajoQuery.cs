using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTRMateriales.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTRMateriales.GetOTRMaterialesByOrdenTrabajo
{
    public record GetOTRMaterialesByOrdenTrabajoQuery(
        string PlantaCode,
        string OrdenTrabajoCode
    ) : IRequest<List<OTRMaterialResponse>>;
}
