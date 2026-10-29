using MediatR;

namespace Cepheus.Application.Features.Comunes.Negocios.ToggleNegocioStatus
{
    public record ToggleNegocioStatusCommand(string Code) : IRequest<bool>;
}
