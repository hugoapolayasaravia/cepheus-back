using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.ToggleFormaPagoVentaStatus
{
    public record ToggleFormaPagoVentaStatusCommand(string Code) : IRequest<bool>;
}
