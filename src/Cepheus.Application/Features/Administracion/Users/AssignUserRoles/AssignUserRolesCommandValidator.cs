using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Users.AssignUserRoles
{
    public class AssignUserRolesCommandValidator : AbstractValidator<AssignUserRolesCommand>
    {
        private readonly IUnitOfWork _uow;

        public AssignUserRolesCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.UserId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .MustAsync(UserExists).WithMessage("El usuario no existe.");

            RuleFor(x => x.RoleIds)
                .NotNull().WithMessage("RoleIds no puede ser null (use una lista vacía para quitar todos los roles).")
                .MustAsync(AllRolesExist).WithMessage("Uno o más de los roles indicados no existen.");
        }

        private async Task<bool> UserExists(int userId, CancellationToken cancellationToken)
            => await _uow.Administracion.Users.Query().AnyAsync(u => u.Id == userId, cancellationToken);

        private async Task<bool> AllRolesExist(List<int> roleIds, CancellationToken cancellationToken)
        {
            if (roleIds is null || roleIds.Count == 0)
            {
                return true; // lista vacía = quitar todos los roles, es válido
            }

            var distinctIds = roleIds.Distinct().ToList();

            var existingCount = await _uow.Administracion.Roles.Query()
                .CountAsync(r => distinctIds.Contains(r.Id), cancellationToken);

            return existingCount == distinctIds.Count;
        }
    }

}
