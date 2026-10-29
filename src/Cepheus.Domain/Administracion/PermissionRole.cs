namespace Cepheus.Domain.Administracion
{
    /// <summary>
    /// Tabla de unión permisos_roles. PK compuesta (RoleId + PermissionId),
    /// mismo patrón que RoleUser (roles_x_users).
    /// </summary>
    public class PermissionRole
    {
        public int RoleId { get; set; }
        public Role Role { get; set; } = default!;

        public int PermissionId { get; set; }
        public Permission Permission { get; set; } = default!;
    }

}
