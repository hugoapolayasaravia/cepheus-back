using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosConsumo.GenerarAsientoConsumo
{
    public class GenerarAsientoConsumoCommandValidator : AbstractValidator<GenerarAsientoConsumoCommand>
    {
        public GenerarAsientoConsumoCommandValidator()
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
