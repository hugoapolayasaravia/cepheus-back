using Cepheus.Application.Features.Administracion.Permissions.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Permissions.GetRolePermissions
{
    public record GetRolePermissionsQuery(int RoleId) : IRequest<List<PermissionResponse>>;
}
