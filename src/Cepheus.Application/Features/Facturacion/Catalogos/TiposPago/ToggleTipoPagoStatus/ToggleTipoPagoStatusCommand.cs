using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.ToggleTipoPagoStatus
{
    public record ToggleTipoPagoStatusCommand(string Code) : IRequest<bool>;
}
