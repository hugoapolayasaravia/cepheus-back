using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Reportes.ComprasAdm.GetReporteComprasAdm;

public sealed class GetReporteComprasAdmQueryValidator : AbstractValidator<GetReporteComprasAdmQuery>
{
    public GetReporteComprasAdmQueryValidator()
    {
        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();
        RuleFor(x => x).Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.");
        RuleFor(x => x.PlantaCode).MaximumLength(2);
    }
}
