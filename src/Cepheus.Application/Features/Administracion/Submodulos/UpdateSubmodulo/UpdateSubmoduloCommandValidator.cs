using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Submodulos.UpdateSubmodulo
{
    public class UpdateSubmoduloCommandValidator : AbstractValidator<UpdateSubmoduloCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateSubmoduloCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del submódulo es obligatorio.")
                .MaximumLength(20)
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un submódulo con ese código dentro de este módulo.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del submódulo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre del submódulo no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un submódulo con ese nombre dentro de este módulo.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para validar concurrencia.");
        }

        private async Task<bool> BeUniqueCode(
            UpdateSubmoduloCommand command, string code, CancellationToken cancellationToken)
        {
            var submodulo = await _uow.Administracion.Submodulos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

            if (submodulo is null)
            {
                return true; // el KeyNotFoundException real lo tira el Handler
            }

            return !await _uow.Administracion.Submodulos.Query()
                .AnyAsync(s =>
                    s.ModuloId == submodulo.ModuloId &&
                    s.Code == code.Trim().ToUpper() &&
                    s.Id != command.Id,
                    cancellationToken);
        }

        private async Task<bool> BeUniqueName(
            UpdateSubmoduloCommand command, string name, CancellationToken cancellationToken)
        {
            var submodulo = await _uow.Administracion.Submodulos.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == command.Id, cancellationToken);

            if (submodulo is null)
            {
                return true;
            }

            return !await _uow.Administracion.Submodulos.Query()
                .AnyAsync(s =>
                    s.ModuloId == submodulo.ModuloId &&
                    s.Name == name.Trim() &&
                    s.Id != command.Id,
                    cancellationToken);
        }
    }

}
