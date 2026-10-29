using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.ToggleEquipoStatus
{
    public record ToggleEquipoStatusCommand(string Code) : IRequest<bool>;
}
