using Microsoft.AspNetCore.Authorization;

namespace Cepheus.Infrastructure.Authorization
{
    /// <summary>
    /// Requisito de autorización basado en Permission/permisos_roles.
    /// Se arma dinámicamente a partir del nombre de política (ver PermissionPolicyProvider),
    /// con la convención "PROGRAMACODE.PERMISSIONCODE" (ej. "USERS.CREATE").
    /// </summary>
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string ProgramaCode { get; }
        public string PermissionCode { get; }

        public PermissionRequirement(string programaCode, string permissionCode)
        {
            ProgramaCode = programaCode;
            PermissionCode = permissionCode;
        }
    }

}
