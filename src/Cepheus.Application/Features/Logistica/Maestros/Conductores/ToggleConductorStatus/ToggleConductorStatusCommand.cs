using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.ToggleConductorStatus
{
    public record ToggleConductorStatusCommand(string Code) : IRequest<bool>;
}
