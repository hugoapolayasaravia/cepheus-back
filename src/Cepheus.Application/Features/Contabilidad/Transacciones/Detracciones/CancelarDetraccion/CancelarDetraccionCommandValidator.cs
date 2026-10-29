using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.CancelarDetraccion
{
    public class CancelarDetraccionCommandValidator : AbstractValidator<CancelarDetraccionCommand>
    {
        public CancelarDetraccionCommandValidator()
        {
            RuleFor(x => x.Year).Must(DetraccionRules.IsValidYear).WithMessage("El año debe tener 4 dígitos.");
            RuleFor(x => x.Period).Must(DetraccionRules.IsValidPeriod).WithMessage("El periodo debe ser un mes entre 01 y 12.");
            RuleFor(x => x.LotNumber).Must(DetraccionRules.IsValidLot).WithMessage("El lote debe tener 4 dígitos.");

            RuleFor(x => x.CancellationDate).NotEmpty().WithMessage("La fecha de cancelación es obligatoria.");

            RuleFor(x => x.PaymentReceiptNumber)
                .NotEmpty().WithMessage("El número de comprobante de pago es obligatorio.")
                .MaximumLength(DetraccionRules.ComprobantePagoMaxLength)
                .WithMessage($"El comprobante de pago no puede exceder los {DetraccionRules.ComprobantePagoMaxLength} caracteres.");
        }
    }
}
