using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.UpdatePrioridad
{
    public record UpdatePrioridadCommand(
        string Code,
        string Name,
        byte[] RowVersion
    ) : IRequest<PrioridadResponse>;
}
