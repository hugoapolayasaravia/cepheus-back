using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Roles.AddRolePermissions
{
    public class AddRolePermissionsCommandValidator : AbstractValidator<AddRolePermissionsCommand>
    {
        private readonly IUnitOfWork _uow;

        public AddRolePermissionsCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.RoleId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .MustAsync(RoleExists).WithMessage("El rol no existe.");

            RuleFor(x => x.PermissionIds)
                .NotNull().WithMessage("PermissionIds no puede ser null.")
                .NotEmpty().WithMessage("Debe indicar al menos un permiso para agregar.")
                .MustAsync(AllPermissionsExist).WithMessage("Uno o más de los permisos indicados no existen.");
        }

        private async Task<bool> RoleExists(int roleId, CancellationToken cancellationToken)
            => await _uow.Administracion.Roles.Query().AnyAsync(r => r.Id == roleId, cancellationToken);

        private async Task<bool> AllPermissionsExist(List<int> permissionIds, CancellationToken cancellationToken)
        {
            var distinctIds = permissionIds.Distinct().ToList();

            var existingCount = await _uow.Administracion.Permissions.Query()
                .CountAsync(p => distinctIds.Contains(p.Id), cancellationToken);

            return existingCount == distinctIds.Count;
        }
    }
}
