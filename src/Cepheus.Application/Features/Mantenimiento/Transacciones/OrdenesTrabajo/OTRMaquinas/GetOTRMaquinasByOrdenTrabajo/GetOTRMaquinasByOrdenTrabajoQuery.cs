using Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTRMaquinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.OTRMaquinas.GetOTRMaquinasByOrdenTrabajo
{
    public record GetOTRMaquinasByOrdenTrabajoQuery(
        string PlantaCode,
        string OrdenTrabajoCode
    ) : IRequest<List<OTRMaquinaResponse>>;
}
