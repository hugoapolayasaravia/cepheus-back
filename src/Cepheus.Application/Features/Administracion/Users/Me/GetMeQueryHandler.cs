using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Comun.Utilidades;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.Me
{
    public class GetMeQueryHandler : IRequestHandler<GetMeQuery, MeResponse>
    {
        // Un programa aparece en el menú cuando el usuario tiene este permiso en él.
        private const string PermisoConsultar = "VIEW";

        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUserService;

        public GetMeQueryHandler(IUnitOfWork uow, ICurrentUserService currentUserService)
        {
            _uow = uow;
            _currentUserService = currentUserService;
        }

        public async Task<MeResponse> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.UserId is null)
            {
                throw new UnauthorizedAccessException("No hay un usuario autenticado.");
            }

            var userId = _currentUserService.UserId.Value;

            var response = await _uow.Administracion.Users.Query()
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new MeResponse
                {
                    Id = u.Id,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList(),
                    AvailableRoles = u.UserRoles
                        .OrderBy(ur => ur.Role.Name)
                        .Select(ur => new MeRoleResponse
                        {
                            Id = ur.Role.Id,
                            Name = ur.Role.Name,
                            Description = ur.Role.Description,
                            IsActive = ur.Role.IsActive
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (response is null)
            {
                throw new KeyNotFoundException("Usuario no encontrado.");
            }

            // Rol activo (el pedido por el cliente si es suyo; si no, el primero por nombre).
            // El menú y los permisos salen SOLO de ese rol: los roles no se suman.
            var activeRoleId = ActiveRole.Pick(
                response.AvailableRoles.Where(r => r.IsActive).Select(r => r.Id).ToList(),
                _currentUserService.RequestedRoleId);
            response.ActiveRoleId = activeRoleId;
            var roleIdVista = activeRoleId ?? -1;

            // Mismas reglas que PermissionAuthorizationHandler (usuario, rol y permiso activos),
            // más que el programa, el submódulo y el módulo estén activos. Una sola consulta.
            var filas = await _uow.Administracion.Users.Query()
                .AsNoTracking()
                .Where(u => u.Id == userId && u.IsActive)
                .SelectMany(u => u.UserRoles)
                .Where(ur => ur.Role.IsActive && ur.RoleId == roleIdVista)
                .SelectMany(ur => ur.Role.PermissionRoles)
                .Where(pr => pr.Permission.IsActive
                    && pr.Permission.Programa.IsActive
                    && pr.Permission.Programa.Submodulo.IsActive
                    && pr.Permission.Programa.Submodulo.Modulo.IsActive)
                .Select(pr => new
                {
                    PermisoCode = pr.Permission.Code,

                    ModuloId = pr.Permission.Programa.Submodulo.Modulo.Id,
                    ModuloCode = pr.Permission.Programa.Submodulo.Modulo.Code,
                    ModuloName = pr.Permission.Programa.Submodulo.Modulo.Name,
                    ModuloIcon = pr.Permission.Programa.Submodulo.Modulo.Icon,
                    ModuloTooltip = pr.Permission.Programa.Submodulo.Modulo.Tooltip,
                    ModuloOrden = pr.Permission.Programa.Submodulo.Modulo.DisplayOrder,

                    SubmoduloId = pr.Permission.Programa.Submodulo.Id,
                    SubmoduloCode = pr.Permission.Programa.Submodulo.Code,
                    SubmoduloName = pr.Permission.Programa.Submodulo.Name,
                    SubmoduloIcon = pr.Permission.Programa.Submodulo.Icon,
                    SubmoduloTooltip = pr.Permission.Programa.Submodulo.Tooltip,
                    SubmoduloOrden = pr.Permission.Programa.Submodulo.DisplayOrder,

                    ProgramaId = pr.Permission.Programa.Id,
                    ProgramaCode = pr.Permission.Programa.Code,
                    ProgramaName = pr.Permission.Programa.Name,
                    ProgramaIcon = pr.Permission.Programa.Icon,
                    ProgramaTooltip = pr.Permission.Programa.Tooltip,
                    ProgramaRoute = pr.Permission.Programa.Route,
                    ProgramaOrden = pr.Permission.Programa.DisplayOrder
                })
                .Distinct()
                .ToListAsync(cancellationToken);

            response.Permissions = filas
                .Select(f => $"{f.ProgramaCode}.{f.PermisoCode}")
                .Distinct()
                .OrderBy(p => p)
                .ToList();

            var permisosPorPrograma = filas
                .GroupBy(f => f.ProgramaId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(f => f.PermisoCode).Distinct().OrderBy(c => c).ToList());

            response.Menu = filas
                .Where(f => f.PermisoCode == PermisoConsultar)
                .GroupBy(f => new { f.ModuloId, f.ModuloCode, f.ModuloName, f.ModuloIcon, f.ModuloTooltip, f.ModuloOrden })
                .OrderBy(m => m.Key.ModuloOrden).ThenBy(m => m.Key.ModuloName)
                .Select(m => new MenuModuloResponse
                {
                    Id = m.Key.ModuloId,
                    Code = m.Key.ModuloCode,
                    Name = m.Key.ModuloName,
                    Icon = m.Key.ModuloIcon,
                    Tooltip = m.Key.ModuloTooltip,
                    DisplayOrder = m.Key.ModuloOrden,
                    Submodulos = m
                        .GroupBy(f => new { f.SubmoduloId, f.SubmoduloCode, f.SubmoduloName, f.SubmoduloIcon, f.SubmoduloTooltip, f.SubmoduloOrden })
                        .OrderBy(s => s.Key.SubmoduloOrden).ThenBy(s => s.Key.SubmoduloName)
                        .Select(s => new MenuSubmoduloResponse
                        {
                            Id = s.Key.SubmoduloId,
                            Code = s.Key.SubmoduloCode,
                            Name = s.Key.SubmoduloName,
                            Icon = s.Key.SubmoduloIcon,
                            Tooltip = s.Key.SubmoduloTooltip,
                            DisplayOrder = s.Key.SubmoduloOrden,
                            Programas = s
                                .OrderBy(f => f.ProgramaOrden).ThenBy(f => f.ProgramaName)
                                .Select(f => new MenuProgramaResponse
                                {
                                    Id = f.ProgramaId,
                                    Code = f.ProgramaCode,
                                    Name = f.ProgramaName,
                                    Icon = f.ProgramaIcon,
                                    Tooltip = f.ProgramaTooltip,
                                    Route = f.ProgramaRoute,
                                    DisplayOrder = f.ProgramaOrden,
                                    Permissions = permisosPorPrograma[f.ProgramaId]
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            return response;
        }
    }
}