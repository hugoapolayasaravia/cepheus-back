using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.ToggleListaPrecioStatus
{
    public record ToggleListaPrecioStatusCommand(long Id) : IRequest<bool>;
}
