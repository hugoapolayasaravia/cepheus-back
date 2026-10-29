using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.CreateCategoriaProducto
{
    public record CreateCategoriaProductoCommand(
        string Name,
        string? FirthCode
    ) : IRequest<CategoriaProductoResponse>;
}
