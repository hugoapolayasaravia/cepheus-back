using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.UpdateDetraccion
{
    public class UpdateDetraccionCommandValidator : AbstractValidator<UpdateDetraccionCommand>
    {
        public UpdateDetraccionCommandValidator()
        {
            DetraccionPeriodRules.Apply(this);

            RuleFor(x => x.LotNumber)
                .Must(DetraccionRules.IsValidLot).WithMessage("El lote debe tener 4 dígitos.");
        }
    }
}
