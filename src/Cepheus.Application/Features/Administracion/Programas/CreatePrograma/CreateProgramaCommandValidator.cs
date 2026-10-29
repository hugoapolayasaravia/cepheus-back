using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Programas.CreatePrograma
{
    public class CreateProgramaCommandValidator : AbstractValidator<CreateProgramaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateProgramaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.SubmoduloId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .MustAsync(SubmoduloExists).WithMessage("El submódulo indicado no existe.");

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
        }

        private async Task<bool> SubmoduloExists(int submoduloId, CancellationToken cancellationToken)
            => await _uow.Administracion.Submodulos.Query().AnyAsync(s => s.Id == submoduloId, cancellationToken);

        private async Task<bool> BeUniqueCode(
            CreateProgramaCommand command, string code, CancellationToken cancellationToken)
            => !await _uow.Administracion.Programas.Query()
                .AnyAsync(p => p.SubmoduloId == command.SubmoduloId && p.Code == code.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueName(
            CreateProgramaCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Administracion.Programas.Query()
                .AnyAsync(p => p.SubmoduloId == command.SubmoduloId && p.Name == name.Trim(), cancellationToken);
    }

}
