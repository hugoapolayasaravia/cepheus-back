using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.ToggleTransportistaStatus
{
    public record ToggleTransportistaStatusCommand(string Code) : IRequest<bool>;
}
