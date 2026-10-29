using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Roles.AssignRolePermissions
{
    public class AssignRolePermissionsCommandValidator : AbstractValidator<AssignRolePermissionsCommand>
    {
        private readonly IUnitOfWork _uow;

        public AssignRolePermissionsCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.RoleId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .MustAsync(RoleExists).WithMessage("El rol no existe.");

            RuleFor(x => x.PermissionIds)
                .NotNull().WithMessage("PermissionIds no puede ser null (use una lista vacía para quitar todos los permisos).")
                .MustAsync(AllPermissionsExist).WithMessage("Uno o más de los permisos indicados no existen.");
        }

        private async Task<bool> RoleExists(int roleId, CancellationToken cancellationToken)
            => await _uow.Administracion.Roles.Query().AnyAsync(r => r.Id == roleId, cancellationToken);

        private async Task<bool> AllPermissionsExist(List<int> permissionIds, CancellationToken cancellationToken)
        {
            if (permissionIds is null || permissionIds.Count == 0)
            {
                return true; // lista vacía = quitar todos los permisos, es válido
            }

            var distinctIds = permissionIds.Distinct().ToList();

            var existingCount = await _uow.Administracion.Permissions.Query()
                .CountAsync(p => distinctIds.Contains(p.Id), cancellationToken);

            return existingCount == distinctIds.Count;
        }
    }

}
