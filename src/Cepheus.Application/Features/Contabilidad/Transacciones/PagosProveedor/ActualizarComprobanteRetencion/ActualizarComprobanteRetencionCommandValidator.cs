using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.ActualizarComprobanteRetencion
{
    public class ActualizarComprobanteRetencionCommandValidator : AbstractValidator<ActualizarComprobanteRetencionCommand>
    {
        public ActualizarComprobanteRetencionCommandValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código de operación es obligatorio.");

            RuleFor(x => x.NumeroComprobanteRetencion)
                .NotEmpty().WithMessage("Comprobante no puede estar en Blanco ...")
                .MaximumLength(10).WithMessage("El comprobante no puede exceder los 10 caracteres.");
        }
    }
}
