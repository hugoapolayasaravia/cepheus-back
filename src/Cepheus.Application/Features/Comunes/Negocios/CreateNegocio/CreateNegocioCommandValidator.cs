using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Negocios.CreateNegocio
{
    public class CreateNegocioCommandValidator : AbstractValidator<CreateNegocioCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateNegocioCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del negocio es obligatorio.")
                .MaximumLength(2).WithMessage("El código no puede exceder los 2 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un negocio con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del negocio es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un negocio con ese nombre.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            var normalizedCode = code.Trim().ToUpper();

            return !await _uow.Comunes.Negocios.Query()
                .AnyAsync(n => n.Code.ToUpper() == normalizedCode, cancellationToken);
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Comunes.Negocios.Query()
                .AnyAsync(n => n.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}
