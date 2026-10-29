using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Permissions.CreatePermission
{
    public class CreatePermissionCommandValidator : AbstractValidator<CreatePermissionCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreatePermissionCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.ProgramaId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .MustAsync(ProgramaExists).WithMessage("El programa indicado no existe.");

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del permiso es obligatorio.")
                .MaximumLength(30)
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un permiso con ese código dentro de este programa.");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre del permiso es obligatorio.")
                .MaximumLength(150);
        }

        private async Task<bool> ProgramaExists(int programaId, CancellationToken cancellationToken)
            => await _uow.Administracion.Programas.Query().AnyAsync(p => p.Id == programaId, cancellationToken);

        private async Task<bool> BeUniqueCode(
            CreatePermissionCommand command, string code, CancellationToken cancellationToken)
            => !await _uow.Administracion.Permissions.Query()
                .AnyAsync(p =>
                    p.ProgramaId == command.ProgramaId && p.Code == code.Trim().ToUpper(),
                    cancellationToken);
    }

}
