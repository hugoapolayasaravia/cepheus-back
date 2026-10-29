using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.GetPrioridadByCode
{
    public record GetPrioridadByCodeQuery(string Code) : IRequest<PrioridadResponse>;
}
