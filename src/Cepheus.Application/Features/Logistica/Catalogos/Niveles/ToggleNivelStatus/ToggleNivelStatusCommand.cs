using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.ToggleNivelStatus
{
    public record ToggleNivelStatusCommand(string Code) : IRequest<bool>;
}
