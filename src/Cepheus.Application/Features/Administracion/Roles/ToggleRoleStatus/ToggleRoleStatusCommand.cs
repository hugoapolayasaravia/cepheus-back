using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.ToggleRoleStatus
{
    public record ToggleRoleStatusCommand(int Id) : IRequest<bool>;
}
