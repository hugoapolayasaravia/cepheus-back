using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.ToggleProductoStatus
{
    public record ToggleProductoStatusCommand(string TipoProductoCode, string Code) : IRequest<bool>;
}
