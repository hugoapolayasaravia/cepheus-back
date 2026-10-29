using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.UpdateCategoriaProducto
{
    public record UpdateCategoriaProductoCommand(
        string Code,
        string Name,
        string? FirthCode,
        byte[] RowVersion
    ) : IRequest<CategoriaProductoResponse>;
}
