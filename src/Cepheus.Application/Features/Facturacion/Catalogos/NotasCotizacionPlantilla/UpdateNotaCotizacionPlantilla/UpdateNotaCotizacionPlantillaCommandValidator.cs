using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Catalogos.NotasCotizacionPlantilla.UpdateNotaCotizacionPlantilla
{
    public class UpdateNotaCotizacionPlantillaCommandValidator : AbstractValidator<UpdateNotaCotizacionPlantillaCommand>
    {
        public UpdateNotaCotizacionPlantillaCommandValidator()
        {
            RuleFor(x => x.NegocioCode).NotEmpty().Length(2);
            RuleFor(x => x.Code).NotEmpty().Length(2);
            RuleFor(x => x.Description).NotEmpty();
            RuleFor(x => x.Option).IsInEnum();
            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
