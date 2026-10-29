using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Utilidades;
using Cepheus.Infrastructure.Persistence.ApplicationDbContexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cepheus.Infrastructure.Authorization
{
    /// <summary>
    /// Valida un PermissionRequirement contra la cadena
    /// User -> UserRole (SOLO el rol activo) -> Role -> PermissionRoles -> Permission -> Programa,
    /// chequeando que el Role, el Permission y el propio User estén activos.
    /// Se ejecuta en cada request que tenga un .RequireAuthorization("X.Y") — es
    /// una query liviana (un solo roundtrip), no cachea nada a propósito: los
    /// cambios de permisos deben reflejarse de inmediato, sin esperar a que
    /// expire el JWT.
    /// </summary>
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly ApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public PermissionAuthorizationHandler(ApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        protected override async Task HandleRequirementAsync(
                AuthorizationHandlerContext context,
                PermissionRequirement requirement)
        {
            // 1. Verificar si el usuario está autenticado
            var isAuthenticated =
                context.User.Identity?.IsAuthenticated ?? false;

            Console.WriteLine($"Autenticado: {isAuthenticated}");

            if (!isAuthenticated)
            {
                return;
            }

            // 2. Obtener el identificador del usuario
            var userIdValue = context.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            Console.WriteLine($"NameIdentifier: {userIdValue}");
            Console.WriteLine($"ProgramaCode: {requirement.ProgramaCode}");
            Console.WriteLine($"PermissionCode: {requirement.PermissionCode}");

            if (!int.TryParse(userIdValue, out var userId))
            {
                return;
            }

            // 3. Rol activo: el usuario trabaja con UN rol a la vez; los permisos de sus
            //    otros roles NO se suman (ver ActiveRole).
            var roleIds = await _context.Users
                .Where(u => u.Id == userId && u.IsActive)
                .SelectMany(u => u.UserRoles)
                .Where(ur => ur.Role.IsActive)
                .OrderBy(ur => ur.Role.Name)
                .Select(ur => ur.RoleId)
                .ToListAsync();

            var activeRoleId = ActiveRole.Pick(roleIds, _currentUser.RequestedRoleId);

            if (activeRoleId is null)
            {
                return;
            }

            // 4. Validar los permisos de ESE rol en la base de datos
            var hasPermission = await _context.Users
                .Where(u => u.Id == userId && u.IsActive)
                .SelectMany(u => u.UserRoles)
                .Where(ur => ur.RoleId == activeRoleId.Value && ur.Role.IsActive)
                .SelectMany(ur => ur.Role.PermissionRoles)
                .AnyAsync(pr =>
                    pr.Permission.IsActive &&
                    pr.Permission.Programa.Code == requirement.ProgramaCode &&
                    pr.Permission.Code == requirement.PermissionCode);

            Console.WriteLine($"Tiene permiso: {hasPermission}");

            if (hasPermission)
            {
                context.Succeed(requirement);
            }
        }
    }

}
