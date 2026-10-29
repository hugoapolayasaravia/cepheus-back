using MediatR;

namespace Cepheus.Application.Features.Comunes.Monedas.ToggleMonedaStatus
{
    public record ToggleMonedaStatusCommand(string Code) : IRequest<bool>;
}
