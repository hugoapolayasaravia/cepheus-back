using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.GetOTRMaterialesByOrdenTrabajo
{
    public record GetOTRMaterialesByOrdenTrabajoQuery(
        string PlantaCode,
        string OrdenTrabajoCode
    ) : IRequest<List<OTRMaterialResponse>>;
}
