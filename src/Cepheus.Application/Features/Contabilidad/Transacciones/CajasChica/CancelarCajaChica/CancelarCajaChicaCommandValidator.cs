using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.CancelarCajaChica
{
    public class CancelarCajaChicaCommandValidator : AbstractValidator<CancelarCajaChicaCommand>
    {
        public CancelarCajaChicaCommandValidator()
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código de la caja es obligatorio.");

            RuleFor(x => x.ReversionYear)
                .Must(y => y!.Length == 4 && y.All(char.IsDigit))
                .WithMessage("El año de la reversión debe tener 4 dígitos.")
                .When(x => !string.IsNullOrWhiteSpace(x.ReversionYear));

            RuleFor(x => x.ReversionMonth)
                .Must(m => int.TryParse(m, out var n) && n >= 1 && n <= 12)
                .WithMessage("El mes de la reversión debe estar entre 01 y 12.")
                .When(x => !string.IsNullOrWhiteSpace(x.ReversionMonth));

            RuleFor(x => x)
                .Must(x => string.IsNullOrWhiteSpace(x.ReversionYear) == string.IsNullOrWhiteSpace(x.ReversionMonth))
                .WithMessage("Indique año y mes de la reversión, o ninguno.")
                .WithName("Reversion");
        }
    }
}
