using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Administracion.Submodulos.CreateSubmodulo
{
    public class CreateSubmoduloCommandValidator : AbstractValidator<CreateSubmoduloCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateSubmoduloCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.ModuloId)
                .Cascade(CascadeMode.Stop)
                .GreaterThan(0)
                .MustAsync(ModuloExists).WithMessage("El módulo indicado no existe.");

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
        }

        private async Task<bool> ModuloExists(int moduloId, CancellationToken cancellationToken)
            => await _uow.Administracion.Modulos.Query().AnyAsync(m => m.Id == moduloId, cancellationToken);

        private async Task<bool> BeUniqueCode(
            CreateSubmoduloCommand command, string code, CancellationToken cancellationToken)
            => !await _uow.Administracion.Submodulos.Query()
                .AnyAsync(s => s.ModuloId == command.ModuloId && s.Code == code.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueName(
            CreateSubmoduloCommand command, string name, CancellationToken cancellationToken)
            => !await _uow.Administracion.Submodulos.Query()
                .AnyAsync(s => s.ModuloId == command.ModuloId && s.Name == name.Trim(), cancellationToken);
    }

}
