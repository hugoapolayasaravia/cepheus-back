using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.GenerarAperturaKardex;

public sealed class GenerarAperturaKardexCommandValidator : AbstractValidator<GenerarAperturaKardexCommand>
{
    public GenerarAperturaKardexCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2)
            .WithMessage("La planta es obligatoria.");
    }
}
