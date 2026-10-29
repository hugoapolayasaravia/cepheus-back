using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.ToggleFormaPagoStatus
{
    public record ToggleFormaPagoStatusCommand(string Code) : IRequest<bool>;
}