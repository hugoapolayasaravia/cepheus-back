using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Vehiculos.ToggleVehiculoStatus
{
    public record ToggleVehiculoStatusCommand(string Code) : IRequest<bool>;
}
