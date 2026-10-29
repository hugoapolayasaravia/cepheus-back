using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.TogglePrioridadStatus
{
    public record TogglePrioridadStatusCommand(string Code) : IRequest<bool>;
}
