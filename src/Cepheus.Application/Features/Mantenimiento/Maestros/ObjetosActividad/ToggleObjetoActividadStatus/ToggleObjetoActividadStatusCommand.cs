using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.ObjetosActividad.ToggleObjetoActividadStatus
{
    public record ToggleObjetoActividadStatusCommand(string Code) : IRequest<bool>;
}
