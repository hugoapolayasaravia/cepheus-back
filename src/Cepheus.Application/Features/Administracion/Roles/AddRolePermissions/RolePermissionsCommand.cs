using Cepheus.Application.Features.Administracion.Permissions.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Roles.AddRolePermissions
{
    /// <summary>
    /// Agrega los PermissionIds recibidos que aún no estén asignados al rol.
    /// Nunca elimina asignaciones existentes. Idempotente: reenviar los mismos
    /// ids no duplica ni falla.
    /// </summary>
    public record AddRolePermissionsCommand(
        int RoleId,
        List<int> PermissionIds
    ) : IRequest<List<PermissionResponse>>;
}
