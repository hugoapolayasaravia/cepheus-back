using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.ToggleCategoriaProductoStatus
{
    public record ToggleCategoriaProductoStatusCommand(string Code) : IRequest<bool>;
}
