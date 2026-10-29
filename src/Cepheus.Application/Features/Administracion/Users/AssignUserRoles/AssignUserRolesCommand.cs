using Cepheus.Application.Features.Administracion.Roles.Common;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.AssignUserRoles
{
    /// <summary>
    /// Reemplazo completo del set de roles del usuario: se agregan los RoleIds
    /// que faltan y se quitan las asignaciones que ya no están en la lista.
    /// Una lista vacía es válida y significa "quitar todos los roles".
    /// </summary>
    public record AssignUserRolesCommand(
        int UserId,
        List<int> RoleIds
    ) : IRequest<List<RoleResponse>>;
}
