using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Monedas.CreateMoneda
{
    public class CreateMonedaCommandValidator : AbstractValidator<CreateMonedaCommand>
    {
        private readonly IUnitOfWork _uow;

        public CreateMonedaCommandValidator(IUnitOfWork uow)
        {
            _uow = uow;

            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El código de la moneda es obligatorio.")
                .Length(3).WithMessage("El código debe tener 3 caracteres (ISO 4217, ej. PEN, USD).")
                .MustAsync(BeUniqueCode).WithMessage("Ya existe una moneda con ese código.");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El nombre de la moneda es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

            RuleFor(x => x.Symbol)
                .MaximumLength(5).WithMessage("El símbolo no puede exceder los 5 caracteres.");

            RuleFor(x => x.NumericCode)
                .Length(3).When(x => !string.IsNullOrWhiteSpace(x.NumericCode))
                .WithMessage("El código numérico ISO 4217 debe tener 3 dígitos.")
                .MustAsync(BeUniqueNumericCode).When(x => !string.IsNullOrWhiteSpace(x.NumericCode))
                .WithMessage("Ya existe una moneda con ese código numérico.");

            RuleFor(x => x.DecimalPlaces)
                .InclusiveBetween(0, 4).WithMessage("Los decimales deben estar entre 0 y 4.");
        }

        private async Task<bool> BeUniqueCode(string code, CancellationToken cancellationToken)
            => !await _uow.Comunes.Monedas.Query()
                .AnyAsync(m => m.Code == code.Trim().ToUpper(), cancellationToken);

        private async Task<bool> BeUniqueNumericCode(string? numericCode, CancellationToken cancellationToken)
            => !await _uow.Comunes.Monedas.Query()
                .AnyAsync(m => m.NumericCode == numericCode!.Trim(), cancellationToken);
    }
}
