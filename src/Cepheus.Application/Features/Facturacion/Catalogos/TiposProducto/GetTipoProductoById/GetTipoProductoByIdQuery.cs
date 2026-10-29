using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.GetTipoProductoById
{
    public record GetTipoProductoByIdQuery(string Code) : IRequest<TipoProductoResponse>;
}
