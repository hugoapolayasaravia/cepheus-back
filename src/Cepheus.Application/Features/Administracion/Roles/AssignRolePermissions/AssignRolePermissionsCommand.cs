using Cepheus.Application.Features.Administracion.Permissions.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.AssignRolePermissions
{
    /// <summary>
    /// Reemplazo completo del set de permisos del rol: se agregan los
    /// PermissionIds que faltan y se quitan las asignaciones que ya no están
    /// en la lista. Una lista vacía es válida (quitar todos los permisos).
    /// </summary>
    public record AssignRolePermissionsCommand(
        int RoleId,
        List<int> PermissionIds
    ) : IRequest<List<PermissionResponse>>;

}
