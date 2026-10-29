using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GenerarAsientosNotaIngresoMasivo
{
    public class GenerarAsientosNotaIngresoMasivoCommandValidator : AbstractValidator<GenerarAsientosNotaIngresoMasivoCommand>
    {
        public GenerarAsientosNotaIngresoMasivoCommandValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2);
            RuleFor(x => x.FechaDesde).NotEmpty().WithMessage("Ingrese la fecha de inicio del proceso.");
            RuleFor(x => x.FechaHasta).NotEmpty().WithMessage("Ingrese la fecha final del proceso.");
            RuleFor(x => x)
                .Must(x => x.FechaDesde <= x.FechaHasta)
                .WithMessage("La fecha de inicio no puede ser posterior a la fecha final.")
                .OverridePropertyName("FechaDesde");
        }
    }
}
