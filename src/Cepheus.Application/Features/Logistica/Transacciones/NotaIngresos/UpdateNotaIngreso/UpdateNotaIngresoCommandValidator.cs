using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.UpdateNotaIngreso;

public sealed class UpdateNotaIngresoCommandValidator : AbstractValidator<UpdateNotaIngresoCommand>
{
    public UpdateNotaIngresoCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(6);
        RuleFor(x => x.ComprobantePagoCode).NotEmpty()
            .WithMessage("El comprobante de pago es obligatorio.");
        RuleFor(x => x.NumeroDocumento).MaximumLength(15);
        RuleFor(x => x.NumeroGuia).MaximumLength(15);
        RuleFor(x => x.FechaEmision).NotEmpty().WithMessage("La fecha de emisión es obligatoria.");
        RuleFor(x => x.FechaRecepcion).NotEmpty().WithMessage("La fecha de recepción es obligatoria.");

        RuleFor(x => x.Igv).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NoGravable).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Renta).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Fonavi).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Servicio).GreaterThanOrEqualTo(0);
        RuleFor(x => x.IgvExterior).GreaterThanOrEqualTo(0);
    }
}
