using Cepheus.Domain.Administracion;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Persistence.Seed
{
    /// <summary>
    /// Agrega los permisos EXPORT_EXCEL y EXPORT_PDF a los programas de Logística (Catálogos y Maestros) que ya existen
    /// (se buscan por Code) y se los asigna al rol "Administrador". Idempotente: se puede
    /// ejecutar en cada arranque. Si un programa aún no existe en la base de datos, se omite.
    /// </summary>
    public static class LogisticaSeeder
    {
        private const string AdministradorRoleName = "Administrador";

        // Code del Programa en la base de datos (el mismo prefijo que usan las políticas).
        private static readonly string[] Programas =
        {
            "COMPRADORES",
            "FAMILIAS",
            "FORMASPAGO",
            "LUGARESENVIO",
            "MOTIVOSDEVOLUCIONART",
            "NIVELES",
            "NOTASCOMPRA",
            "PLANESARTICULO",
            "SUBFAMILIAS",
            "TIPOSARTICULO",
            "TIPOSCOMPRA",
            "TIPOSPEDIDO",
            "TIPOSTRANSACCION",
            "TIPOSVALE",
            "TRAMITES",
            "UNIDADESMEDIDA",
            "UNIDADESNEGOCIO",
            "ARTICULOS",
            "PROVEEDORES"
        };

        private static readonly (string Code, string Name)[] Permisos =
        {
            ("EXPORT_EXCEL", "Exportar a Excel"),
            ("EXPORT_PDF", "Exportar a PDF")
        };

        public static async Task SeedAsync(ApplicationDbContext context, CancellationToken cancellationToken = default)
        {
            var programas = await context.Programas
                .Where(p => Programas.Contains(p.Code))
                .ToListAsync(cancellationToken);

            var ids = new List<int>();

            foreach (var programa in programas)
            {
                foreach (var (code, name) in Permisos)
                {
                    var permission = await context.Permissions
                        .FirstOrDefaultAsync(p => p.ProgramaId == programa.Id && p.Code == code, cancellationToken);

                    if (permission is null)
                    {
                        permission = new Permission
                        {
                            ProgramaId = programa.Id,
                            Code = code,
                            Name = name,
                            IsActive = true
                        };
                        context.Permissions.Add(permission);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    ids.Add(permission.Id);
                }
            }

            var adminRole = await context.Roles
                .FirstOrDefaultAsync(r => r.Name == AdministradorRoleName, cancellationToken);
            if (adminRole is null || ids.Count == 0) return;

            var assigned = await context.PermissionRoles
                .Where(pr => pr.RoleId == adminRole.Id)
                .Select(pr => pr.PermissionId)
                .ToListAsync(cancellationToken);

            foreach (var id in ids.Except(assigned).Distinct())
            {
                context.PermissionRoles.Add(new PermissionRole { RoleId = adminRole.Id, PermissionId = id });
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
