using Cepheus.Application.Features.Administracion.Roles.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.UpdateRole
{
    public record UpdateRoleCommand(
        int Id,
        string Name,
        string? Description,
        byte[] RowVersion
    ) : IRequest<RoleResponse>;

}
