using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.ProcesarCierreMensual;

public sealed class ProcesarCierreMensualCommandValidator : AbstractValidator<ProcesarCierreMensualCommand>
{
    public ProcesarCierreMensualCommandValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2)
            .WithMessage("La planta es obligatoria.");

        RuleFor(x => x.PeriodoCode)
            .Must(p => ControlCierreRules.IsValidPeriod(p))
            .When(x => !string.IsNullOrWhiteSpace(x.PeriodoCode))
            .WithMessage("El período debe tener el formato AAAAMM (ej. 202609).");
    }
}
