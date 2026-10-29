namespace Cepheus.Application.Features.Administracion.Users.Me
{
    public class MeResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public List<string> Roles { get; set; } = new();

        /// <summary>Todos los roles del usuario (con id), para el selector de rol del Header.</summary>
        public List<MeRoleResponse> AvailableRoles { get; set; } = new();

        /// <summary>Rol activo: Menu y Permissions se calculan SOLO con este rol (nunca se suman roles).</summary>
        public int? ActiveRoleId { get; set; }

        /// <summary>
        /// Menú del usuario: Módulo -> Submódulo -> Programa. Solo incluye lo activo y los
        /// programas donde el usuario tiene el permiso VIEW. Deliberadamente NO va en el JWT
        /// (ver Authenticate): se calcula en cada consulta, así los cambios de permisos se
        /// reflejan de inmediato.
        /// </summary>
        public List<MenuModuloResponse> Menu { get; set; } = new();

        /// <summary>
        /// Lista plana de permisos con el mismo formato de las políticas de los endpoints,
        /// por ejemplo "USERS.VIEW" o "ROLES.UPDATE".
        /// </summary>
        public List<string> Permissions { get; set; } = new();
    }

    public class MeRoleResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}