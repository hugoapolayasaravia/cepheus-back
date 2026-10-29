using Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.GetOTResponsablesByOrdenTrabajo
{
    public record GetOTResponsablesByOrdenTrabajoQuery(
        string PlantaCode,
        string OrdenTrabajoCode
    ) : IRequest<List<OTResponsableResponse>>;
}
