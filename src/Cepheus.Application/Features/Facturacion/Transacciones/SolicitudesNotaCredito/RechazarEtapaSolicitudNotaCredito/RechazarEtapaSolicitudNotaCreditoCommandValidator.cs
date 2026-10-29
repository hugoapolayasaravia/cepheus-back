using Cepheus.Application.Features.Facturacion.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.SolicitudesNotaCredito.RechazarEtapaSolicitudNotaCredito
{
    public class RechazarEtapaSolicitudNotaCreditoCommandValidator : AbstractValidator<RechazarEtapaSolicitudNotaCreditoCommand>
    {
        public RechazarEtapaSolicitudNotaCreditoCommandValidator()
        {
            RuleFor(x => x.Year).NotEmpty().Length(4).WithMessage("El año debe tener 4 caracteres.");
            RuleFor(x => x.Month).NotEmpty().Length(2).WithMessage("El mes debe tener 2 caracteres.");
            RuleFor(x => x.Code).NotEmpty().WithMessage("El código de la solicitud es obligatorio.");

            RuleFor(x => x.Stage)
                .Must(s => EtapaSolicitudNotaCreditoParser.TryParse(s, out _))
                .WithMessage($"La etapa debe ser una de: {EtapaSolicitudNotaCreditoParser.ValidNames}.");

            RuleFor(x => x.Observation)
                .MaximumLength(200).WithMessage("La observación no puede exceder los 200 caracteres.");

            RuleFor(x => x.Observation)
                .NotEmpty().WithMessage("Debe indicar el motivo del rechazo en la observación.");
        }
    }
}
