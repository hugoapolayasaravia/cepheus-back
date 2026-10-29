using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.ImportarConstanciasDetraccion
{
    public class ImportarConstanciasDetraccionCommandValidator : AbstractValidator<ImportarConstanciasDetraccionCommand>
    {
        public ImportarConstanciasDetraccionCommandValidator()
        {
            RuleFor(x => x.Year).Must(DetraccionRules.IsValidYear).WithMessage("El año debe tener 4 dígitos.");
            RuleFor(x => x.Period).Must(DetraccionRules.IsValidPeriod).WithMessage("El periodo debe ser un mes entre 01 y 12.");
            RuleFor(x => x.LotNumber).Must(DetraccionRules.IsValidLot).WithMessage("El lote debe tener 4 dígitos.");
            RuleFor(x => x.Content).NotEmpty().WithMessage("El archivo de constancias está vacío.");
        }
    }
}
