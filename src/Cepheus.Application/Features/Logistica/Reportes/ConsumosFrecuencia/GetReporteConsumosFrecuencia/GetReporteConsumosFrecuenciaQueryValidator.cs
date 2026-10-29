using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosFrecuencia.GetReporteConsumosFrecuencia;

public sealed class GetReporteConsumosFrecuenciaQueryValidator : AbstractValidator<GetReporteConsumosFrecuenciaQuery>
{
    public GetReporteConsumosFrecuenciaQueryValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2).WithMessage("La planta es obligatoria.");
        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();
        RuleFor(x => x).Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.");
    }
}
