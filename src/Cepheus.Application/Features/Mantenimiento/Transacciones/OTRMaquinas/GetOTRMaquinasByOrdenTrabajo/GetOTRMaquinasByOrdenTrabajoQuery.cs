using Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.GetOTRMaquinasByOrdenTrabajo
{
    public record GetOTRMaquinasByOrdenTrabajoQuery(
        string PlantaCode,
        string OrdenTrabajoCode
    ) : IRequest<List<OTRMaquinaResponse>>;
}
