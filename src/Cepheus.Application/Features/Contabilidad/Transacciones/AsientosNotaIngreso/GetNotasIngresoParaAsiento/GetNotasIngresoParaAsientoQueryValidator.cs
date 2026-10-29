using FluentValidation;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.GetNotasIngresoParaAsiento
{
    public class GetNotasIngresoParaAsientoQueryValidator : AbstractValidator<GetNotasIngresoParaAsientoQuery>
    {
        public GetNotasIngresoParaAsientoQueryValidator()
        {
            RuleFor(x => x.PlantaCode).NotEmpty().WithMessage("La planta es obligatoria.");
            RuleFor(x => x.FechaDesde).NotNull().WithMessage("Ingrese la fecha de inicio del proceso.");
            RuleFor(x => x.FechaHasta).NotNull().WithMessage("Ingrese la fecha final del proceso.");
            RuleFor(x => x)
                .Must(x => !x.FechaDesde.HasValue || !x.FechaHasta.HasValue || x.FechaDesde.Value <= x.FechaHasta.Value)
                .WithMessage("La fecha de inicio no puede ser posterior a la fecha final.")
                .OverridePropertyName("FechaDesde");
        }
    }
}
