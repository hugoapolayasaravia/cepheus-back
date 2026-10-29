using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.CreatePrioridad
{
    public record CreatePrioridadCommand(
        string Code,
        string Name
    ) : IRequest<PrioridadResponse>;
}
