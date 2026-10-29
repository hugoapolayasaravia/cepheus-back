using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.ToggleCentroEjecutorStatus
{
    public record ToggleCentroEjecutorStatusCommand(string Code) : IRequest<bool>;
}
