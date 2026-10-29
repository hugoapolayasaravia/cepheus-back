using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.ToggleActividadStatus
{
    public record ToggleActividadStatusCommand(string Code) : IRequest<bool>;
}
