using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.VerbosActividad.ToggleVerboActividadStatus
{
    public record ToggleVerboActividadStatusCommand(string Code) : IRequest<bool>;
}
