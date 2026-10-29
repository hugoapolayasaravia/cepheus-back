using Cepheus.Application.Features.Comunes.TiposCambio.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposCambio.GetTipoCambioById
{
    public record GetTipoCambioByIdQuery(int Id) : IRequest<TipoCambioResponse>;
}
