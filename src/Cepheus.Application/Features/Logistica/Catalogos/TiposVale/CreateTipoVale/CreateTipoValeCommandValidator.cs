using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposVale.CreateTipoVale
{
    public class CreateTipoValeCommandValidator : AbstractValidator<CreateTipoValeCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateTipoValeCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código del tipo de vale es obligatorio.")
                .MaximumLength(3).WithMessage("El código no puede exceder 3 caracteres.")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe un tipo de vale con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre del tipo de vale es obligatorio.")
                .MaximumLength(50).WithMessage("El nombre no puede exceder los 50 caracteres.")
                .MustAsync(BeUniqueName).WithMessage("Ya existe un tipo de vale con ese nombre.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
        {
            var normalizedCode = code.Trim().ToUpper();
            return !await _uow.Logistica.Catalogos.TiposVale.Query()
                .AnyAsync(x => x.Code.ToUpper() == normalizedCode, cancellationToken);
        }

        private async Task<bool> BeUniqueName(string name, CancellationToken cancellationToken)
            => !await _uow.Logistica.Catalogos.TiposVale.Query()
                .AnyAsync(t => t.Name.ToLower() == name.Trim().ToLower(), cancellationToken);
    }
}