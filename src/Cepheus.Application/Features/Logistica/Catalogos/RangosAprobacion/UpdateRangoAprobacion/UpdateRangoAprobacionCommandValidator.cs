using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.UpdateRangoAprobacion
{
    public class UpdateRangoAprobacionCommandValidator : AbstractValidator<UpdateRangoAprobacionCommand>
    {
        public UpdateRangoAprobacionCommandValidator()
        {
            RuleFor(x => x.NivelCode).NotEmpty();
            RuleFor(x => x.TipoTransaccionCode).NotEmpty();
            RuleFor(x => x.UnidadNegocioCode).NotEmpty();
            RuleFor(x => x.MonedaCode).NotEmpty();

            RuleFor(x => x.ImporteMinimo).GreaterThanOrEqualTo(0).WithMessage("El importe mínimo no puede ser negativo.");

            RuleFor(x => x.ImporteMaximo)
                .GreaterThanOrEqualTo(x => x.ImporteMinimo)
                .WithMessage("El importe máximo no puede ser menor al importe mínimo.");

            RuleFor(x => x.ImporteAcumuladoDiario).GreaterThanOrEqualTo(0).WithMessage("El acumulado diario no puede ser negativo.");
            RuleFor(x => x.ImporteAcumuladoMensual).GreaterThanOrEqualTo(0).WithMessage("El acumulado mensual no puede ser negativo.");

            RuleFor(x => x.PorcentajeTotal)
                .InclusiveBetween(0, 100).WithMessage("El porcentaje total debe estar entre 0 y 100.")
                .When(x => x.PorcentajeTotal.HasValue);

            RuleFor(x => x.RowVersion).NotEmpty().WithMessage("RowVersion es obligatorio para control de concurrencia.");
        }
    }
}
