using MediatR;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.ToggleComprobantePagoStatus
{
    public record ToggleComprobantePagoStatusCommand(string Code) : IRequest<bool>;
}
