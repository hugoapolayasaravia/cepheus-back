using Cepheus.Domain.Administracion;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Siembra el árbol Modulo("ADMIN") -> Submodulo("SEG") -> Programa(USERS, ROLES,
    /// MODULOS, SUBMODULOS, PROGRAMAS, PERMISSIONS), cada uno con su CRUD estándar
    /// (VIEW/CREATE/UPDATE/DELETE) en Permission. Sin esto, los propios endpoints de
    /// administración no tendrían ningún Permission contra el cual autorizar.
    ///
    /// Idempotente: se puede correr en cada arranque de la app sin duplicar nada
    /// (busca por Code antes de crear). Al final, sincroniza el rol "Administrador"
    /// para que tenga TODOS los permisos sembrados — así el admin nunca queda
    /// bloqueado de su propio sistema, ni siquiera si se agregan Programas nuevos acá
    /// en el futuro (ej. cuando se sume Logística).
    /// </summary>
    public static class AdministracionSeeder
    {
        private const string ModuloCode = "ADMIN";
        private const string ModuloName = "Administración";
        private const string SubmoduloCode = "SEG";
        private const string SubmoduloName = "Seguridad";
        private const string AdministradorRoleName = "Administrador";

        private static readonly (string Code, string Name)[] Programas =
        {
        ("USERS", "Usuarios"),
        ("ROLES", "Roles"),
        ("MODULOS", "Módulos"),
        ("SUBMODULOS", "Submódulos"),
        ("PROGRAMAS", "Programas"),
        ("PERMISSIONS", "Permisos")
    };

        private static readonly (string Code, string Name)[] StandardPermissions =
        {
        ("VIEW", "Consultar"),
        ("CREATE", "Crear"),
        ("UPDATE", "Modificar"),
        ("DELETE", "Eliminar"),
        ("EXPORT_PDF", "Exportar a PDF"),
        ("EXPORT_EXCEL", "Exportar a Excel")
    };

        public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
        {
            var modulo = await GetOrCreateModuloAsync(context, cancellationToken);
            var submodulo = await GetOrCreateSubmoduloAsync(context, modulo.Id, cancellationToken);

            var allSeededPermissionIds = new List<int>();

            foreach (var (programaCode, programaName) in Programas)
            {
                var programa = await GetOrCreateProgramaAsync(context, submodulo.Id, programaCode, programaName, cancellationToken);

                foreach (var (permissionCode, permissionName) in StandardPermissions)
                {
                    var permission = await GetOrCreatePermissionAsync(
                        context, programa.Id, permissionCode, permissionName, cancellationToken);

                    allSeededPermissionIds.Add(permission.Id);
                }
            }

            await SyncAdministradorRoleAsync(context, allSeededPermissionIds, cancellationToken);
        }

        private static async Task<Modulo> GetOrCreateModuloAsync(
            ApplicationDbContext context, CancellationToken cancellationToken)
        {
            var modulo = await context.Modulos
                .FirstOrDefaultAsync(m => m.Code == ModuloCode, cancellationToken);

            if (modulo is not null)
            {
                return modulo;
            }

            modulo = new Modulo
            {
                Code = ModuloCode,
                Name = ModuloName,
                Icon = "shield",
                Tooltip = "Administración del sistema",
                DisplayOrder = 1,
                IsActive = true
            };

            context.Modulos.Add(modulo);
            await context.SaveChangesAsync(cancellationToken);

            return modulo;
        }

        private static async Task<Submodulo> GetOrCreateSubmoduloAsync(
            ApplicationDbContext context, int moduloId, CancellationToken cancellationToken)
        {
            var submodulo = await context.Submodulos
                .FirstOrDefaultAsync(s => s.ModuloId == moduloId && s.Code == SubmoduloCode, cancellationToken);

            if (submodulo is not null)
            {
                return submodulo;
            }

            submodulo = new Submodulo
            {
                ModuloId = moduloId,
                Code = SubmoduloCode,
                Name = SubmoduloName,
                Icon = "lock",
                Tooltip = "Usuarios, roles y estructura de menú",
                DisplayOrder = 0,
                IsActive = true
            };

            context.Submodulos.Add(submodulo);
            await context.SaveChangesAsync(cancellationToken);

            return submodulo;
        }

        private static async Task<Programa> GetOrCreateProgramaAsync(
            ApplicationDbContext context, int submoduloId, string code, string name, CancellationToken cancellationToken)
        {
            var programa = await context.Programas
                .FirstOrDefaultAsync(p => p.SubmoduloId == submoduloId && p.Code == code, cancellationToken);

            if (programa is not null)
            {
                return programa;
            }

            programa = new Programa
            {
                SubmoduloId = submoduloId,
                Code = code,
                Name = name,
                Icon = "list",
                Tooltip = "Ver listado de " + name,
                Route = RutaDe(code),
                DisplayOrder = 1,
                IsActive = true
            };

            context.Programas.Add(programa);
            await context.SaveChangesAsync(cancellationToken);

            return programa;
        }

        // Ruta de la pantalla en el front (debe coincidir con administracion.routes.ts); de ahí salen el menú y los breadcrumbs.
        private static string RutaDe(string programaCode) => programaCode switch
        {
            "USERS" => "/administracion/usuarios",
            "ROLES" => "/administracion/roles",
            "MODULOS" => "/administracion/modulos",
            "SUBMODULOS" => "/administracion/submodulos",
            "PROGRAMAS" => "/administracion/programas",
            "PERMISSIONS" => "/administracion/permisos",
            _ => "/route/ruta"
        };

        private static async Task<Permission> GetOrCreatePermissionAsync(
            ApplicationDbContext context, int programaId, string code, string name, CancellationToken cancellationToken)
        {
            var permission = await context.Permissions
                .FirstOrDefaultAsync(p => p.ProgramaId == programaId && p.Code == code, cancellationToken);

            if (permission is not null)
            {
                return permission;
            }

            permission = new Permission
            {
                ProgramaId = programaId,
                Code = code,
                Name = name,
                IsActive = true
            };

            context.Permissions.Add(permission);
            await context.SaveChangesAsync(cancellationToken);

            return permission;
        }

        private static async Task SyncAdministradorRoleAsync(
            ApplicationDbContext context, List<int> seededPermissionIds, CancellationToken cancellationToken)
        {
            var adminRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == AdministradorRoleName, cancellationToken);

            // Si todavía no existe (nadie hizo /register), no hay nada que sincronizar.
            // Register ya crea el rol y se autoasigna todo lo que exista en ese momento;
            // esto cubre el caso de agregar Programas nuevos DESPUÉS del bootstrap inicial.
            if (adminRole is null)
            {
                return;
            }

            var alreadyAssignedIds = await context.PermissionRoles
                .Where(pr => pr.RoleId == adminRole.Id)
                .Select(pr => pr.PermissionId)
                .ToListAsync(cancellationToken);

            var missingIds = seededPermissionIds.Except(alreadyAssignedIds).Distinct();

            foreach (var permissionId in missingIds)
            {
                context.PermissionRoles.Add(new PermissionRole
                {
                    RoleId = adminRole.Id,
                    PermissionId = permissionId
                });
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }

}
