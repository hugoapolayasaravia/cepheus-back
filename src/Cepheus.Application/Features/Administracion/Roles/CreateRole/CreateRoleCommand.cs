using Cepheus.Application.Features.Administracion.Roles.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.CreateRole
{
    public record CreateRoleCommand(
    string Name,
    string? Description
) : IRequest<RoleResponse>;
}
