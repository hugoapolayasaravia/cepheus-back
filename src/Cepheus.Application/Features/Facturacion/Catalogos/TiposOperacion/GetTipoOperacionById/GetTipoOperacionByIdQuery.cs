using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.GetTipoOperacionById
{
    public record GetTipoOperacionByIdQuery(string Code) : IRequest<TipoOperacionResponse>;
}
