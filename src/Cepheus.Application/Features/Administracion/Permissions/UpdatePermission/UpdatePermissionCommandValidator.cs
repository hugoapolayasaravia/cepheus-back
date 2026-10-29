using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Permissions.UpdatePermission
{
    public class UpdatePermissionCommandValidator : AbstractValidator<UpdatePermissionCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePermissionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del permiso es obligatorio.")
                .MaximumLength(30)
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un permiso con ese código dentro de este programa.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del permiso es obligatorio.")
                .MaximumLength(150);

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para validar concurrencia.");
        }

        private async Task<bool> BeUniqueCode(
            UpdatePermissionCommand command, string code, CancellationToken cancellationToken)
        {
            var permission = await _uow.Administracion.Permissions.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

            if (permission is null)
            {
                return true; // el KeyNotFoundException real lo tira el Handler
            }

            return !await _uow.Administracion.Permissions.Query()
                .AnyAsync(p =>
                    p.ProgramaId == permission.ProgramaId &&
                    p.Code == code.Trim().ToUpper() &&
                    p.Id != command.Id,
                    cancellationToken);
        }
    }

}
