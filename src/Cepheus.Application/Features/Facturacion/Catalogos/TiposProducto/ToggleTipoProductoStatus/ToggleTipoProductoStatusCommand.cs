using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.ToggleTipoProductoStatus
{
    public record ToggleTipoProductoStatusCommand(string Code) : IRequest<bool>;
}
