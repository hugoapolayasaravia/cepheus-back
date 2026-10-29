using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosAnexos.GetReporteConsumosAnexos;

public sealed class GetReporteConsumosAnexosQueryValidator : AbstractValidator<GetReporteConsumosAnexosQuery>
{
    public GetReporteConsumosAnexosQueryValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2).WithMessage("La planta es obligatoria.");
        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();
        RuleFor(x => x).Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.");
    }
}
