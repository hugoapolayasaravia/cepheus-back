using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.GetTipoValorizacionByCode
{
    public record GetTipoValorizacionByCodeQuery(string Code) : IRequest<TipoValorizacionResponse>;
}
