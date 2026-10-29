using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Programas.UpdatePrograma
{
    public class UpdateProgramaCommandValidator : AbstractValidator<UpdateProgramaCommand>
    {
        private readonly IUnitOfWork _uow;

        public UpdateProgramaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del programa es obligatorio.")
                .MaximumLength(20)
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un programa con ese código dentro de este submódulo.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del programa es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre del programa no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un programa con ese nombre dentro de este submódulo.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.RowVersion)
                .NotEmpty().WithMessage("RowVersion es obligatorio para validar concurrencia.");
        }

        private async Task<bool> BeUniqueCode(
            UpdateProgramaCommand command, string code, CancellationToken cancellationToken)
        {
            var programa = await _uow.Administracion.Programas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

            if (programa is null)
            {
                return true;
            }

            return !await _uow.Administracion.Programas.Query()
                .AnyAsync(p =>
                    p.SubmoduloId == programa.SubmoduloId &&
                    p.Code == code.Trim().ToUpper() &&
                    p.Id != command.Id,
                    cancellationToken);
        }

        private async Task<bool> BeUniqueName(
            UpdateProgramaCommand command, string name, CancellationToken cancellationToken)
        {
            var programa = await _uow.Administracion.Programas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == command.Id, cancellationToken);

            if (programa is null)
            {
                return true;
            }

            return !await _uow.Administracion.Programas.Query()
                .AnyAsync(p =>
                    p.SubmoduloId == programa.SubmoduloId &&
                    p.Name == name.Trim() &&
                    p.Id != command.Id,
                    cancellationToken);
        }
    }

}
