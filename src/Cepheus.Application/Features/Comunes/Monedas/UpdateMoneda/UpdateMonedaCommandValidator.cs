using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.Monedas.UpdateMoneda
{
    public class UpdateMonedaCommandValidator : AbstractValidator<UpdateMonedaCommand>
    {
        public UpdateMonedaCommandValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.Code)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El código de la moneda es obligatorio.")
                .Length(3)
                .WithMessage("El código debe tener 3 caracteres (ISO 4217).");

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("El nombre de la moneda es obligatorio.")
                .MaximumLength(100);

            RuleFor(x => x.Symbol)
                .MaximumLength(5);

            RuleFor(x => x.NumericCode)
                .Length(3)
                .When(x => !string.IsNullOrWhiteSpace(x.NumericCode))
                .WithMessage("El código numérico ISO 4217 debe tener 3 dígitos.")
                .MustAsync(async (command, numericCode, cancellationToken) =>
                    !await uow.Comunes.Monedas.Query()
                        .AnyAsync(
                            m => m.NumericCode == numericCode!.Trim()
                                 && m.Code != command.Code.Trim().ToUpperInvariant(),
                            cancellationToken))
                .When(x => !string.IsNullOrWhiteSpace(x.NumericCode))
                .WithMessage("Ya existe una moneda con ese código numérico.");

            RuleFor(x => x.DecimalPlaces)
                .InclusiveBetween(0, 4)
                .WithMessage("Los decimales deben estar entre 0 y 4.");
        }
    }
}