using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.UpdateDetraccionDocumentoConstancia
{
    public class UpdateDetraccionDocumentoConstanciaCommandValidator
        : AbstractValidator<UpdateDetraccionDocumentoConstanciaCommand>
    {
        public UpdateDetraccionDocumentoConstanciaCommandValidator()
        {
            RuleFor(x => x.Year).Must(DetraccionRules.IsValidYear).WithMessage("El año debe tener 4 dígitos.");
            RuleFor(x => x.Period).Must(DetraccionRules.IsValidPeriod).WithMessage("El periodo debe ser un mes entre 01 y 12.");
            RuleFor(x => x.LotNumber).Must(DetraccionRules.IsValidLot).WithMessage("El lote debe tener 4 dígitos.");

            RuleFor(x => x.OperacionDetraccion)
                .MaximumLength(DetraccionRules.OperacionMaxLength)
                .WithMessage($"La operación no puede exceder los {DetraccionRules.OperacionMaxLength} caracteres.");

            RuleFor(x => x.ConstanciaDetraccion)
                .MaximumLength(DetraccionRules.ConstanciaMaxLength)
                .WithMessage($"La constancia no puede exceder los {DetraccionRules.ConstanciaMaxLength} caracteres.");
        }
    }
}
