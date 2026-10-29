using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTResponsables.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTResponsables.GetOTResponsablesByOrdenTrabajo
{
    public record GetOTResponsablesByOrdenTrabajoQuery(
        string PlantaCode,
        string OrdenTrabajoCode
    ) : IRequest<List<OTResponsableResponse>>;
}
