using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Administracion.Permissions.Common;
using Cepheus.Domain.Administracion;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Roles.AssignRolePermissions
{
    public class AssignRolePermissionsCommandHandler
    : IRequestHandler<AssignRolePermissionsCommand, List<PermissionResponse>>
    {
        private readonly IUnitOfWork _uow;

        public AssignRolePermissionsCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<List<PermissionResponse>> Handle(
            AssignRolePermissionsCommand request, CancellationToken cancellationToken)
        {
            // Existencia de Role y de todos los Permissions ya se validó en el Validator.
            var requestedPermissionIds = request.PermissionIds.Distinct().ToHashSet();

            var currentAssignments = await _uow.Administracion.PermissionRoles.Query()
                .Where(pr => pr.RoleId == request.RoleId)
                .ToListAsync(cancellationToken);

            var currentPermissionIds = currentAssignments.Select(pr => pr.PermissionId).ToHashSet();

            // Reemplazo completo (como AssignUserRoles): se quitan las asignaciones que ya no están en la lista...
            var toRemove = currentAssignments
                .Where(pr => !requestedPermissionIds.Contains(pr.PermissionId))
                .ToList();

            foreach (var assignment in toRemove)
            {
                _uow.Administracion.PermissionRoles.Remove(assignment);
            }

            // ...y se agregan las que faltan.
            var toAddIds = requestedPermissionIds
                .Where(permissionId => !currentPermissionIds.Contains(permissionId))
                .ToList();

            foreach (var permissionId in toAddIds)
            {
                await _uow.Administracion.PermissionRoles.AddAsync(
                    new PermissionRole { RoleId = request.RoleId, PermissionId = permissionId },
                    cancellationToken);
            }

            if (toAddIds.Count > 0 || toRemove.Count > 0)
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }

            return await _uow.Administracion.PermissionRoles.Query()
                .Where(pr => pr.RoleId == request.RoleId)
                .Select(pr => new PermissionResponse
                {
                    Id = pr.Permission.Id,
                    ProgramaId = pr.Permission.ProgramaId,
                    Code = pr.Permission.Code,
                    Name = pr.Permission.Name,
                    IsActive = pr.Permission.IsActive,
                    CreatedAt = pr.Permission.CreatedAt,
                    UpdatedAt = pr.Permission.UpdatedAt,
                    RowVersion = pr.Permission.RowVersion
                })
                .OrderBy(p => p.Code)
                .ToListAsync(cancellationToken);
        }
    }

}
