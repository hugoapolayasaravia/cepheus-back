using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Transferencias.GenerarTransferencia;

public sealed class GenerarTransferenciaCommandValidator : AbstractValidator<GenerarTransferenciaCommand>
{
    public GenerarTransferenciaCommandValidator()
    {
        RuleFor(x => x.PlantaDestinoCode).NotEmpty().MaximumLength(2)
            .WithMessage("La planta destino es obligatoria.");

        RuleFor(x => x.PlantaOrigenCode).NotEmpty().MaximumLength(2)
            .WithMessage("La planta origen es obligatoria.");

        RuleFor(x => x.ValeCode).NotEmpty().MaximumLength(7)
            .WithMessage("El código del vale es obligatorio.");

        RuleFor(x => x.FechaRecepcion)
            .NotEqual(default(DateTime)).WithMessage("La fecha de recepción es obligatoria.");
    }
}
