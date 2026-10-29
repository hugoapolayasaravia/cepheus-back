using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
namespace Cepheus.Application.Features.Administracion.Modulos.CreateModulo
{
    public class CreateModuloCommandValidator : AbstractValidator<CreateModuloCommand>
    {
        private readonly IUnitOfWork _uow;
        public CreateModuloCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;
            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del módulo es obligatorio.")
                .MaximumLength(20).WithMessage("El código del módulo no puede exceder los 20 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un módulo con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del módulo es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre del módulo no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un módulo con ese nombre.");

            RuleFor(x => x.DisplayOrder)
                .GreaterThanOrEqualTo(0).WithMessage("El orden de visualización debe ser mayor o igual a 0.");

            RuleFor(x => x.Tooltip)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El tooltip del módulo es obligatorio.")
                .MaximumLength(200).WithMessage("El tooltip no puede exceder los 200 caracteres.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
            => !await _uow.Administracion.Modulos.Query()
                .AnyAsync(m => m.Code == code.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Administracion.Modulos.Query()
                .AnyAsync(m => m.Name == name.Trim(), cancellationToken);
    }
}