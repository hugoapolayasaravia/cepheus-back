using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.GetCategoriaProductoByCode
{
    public record GetCategoriaProductoByCodeQuery(string Code) : IRequest<CategoriaProductoResponse>;
}
