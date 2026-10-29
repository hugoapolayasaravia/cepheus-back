using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.AnularCotizacion
{
    public class AnularCotizacionCommandValidator : AbstractValidator<AnularCotizacionCommand>
    {
        public AnularCotizacionCommandValidator()
        {
            RuleFor(x => x.NegocioCode).NotEmpty().Length(2);
            RuleFor(x => x.Year).NotEmpty().Length(4);
            RuleFor(x => x.Month).NotEmpty().Length(2);
            RuleFor(x => x.Code).NotEmpty().Length(8);
            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("El motivo de anulación es obligatorio.")
                .MaximumLength(500);
        }
    }
}
