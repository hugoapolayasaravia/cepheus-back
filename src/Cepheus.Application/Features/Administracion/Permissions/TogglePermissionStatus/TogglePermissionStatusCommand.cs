using MediatR;

namespace Cepheus.Application.Features.Administracion.Permissions.TogglePermissionStatus
{
    public record TogglePermissionStatusCommand(int Id) : IRequest<bool>;
}
