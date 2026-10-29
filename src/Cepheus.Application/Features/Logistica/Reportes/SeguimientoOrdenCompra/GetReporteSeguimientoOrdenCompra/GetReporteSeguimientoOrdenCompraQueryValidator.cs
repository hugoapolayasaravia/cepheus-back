using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Reportes.SeguimientoOrdenCompra.GetReporteSeguimientoOrdenCompra;

public sealed class GetReporteSeguimientoOrdenCompraQueryValidator
    : AbstractValidator<GetReporteSeguimientoOrdenCompraQuery>
{
    public GetReporteSeguimientoOrdenCompraQueryValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2).WithMessage("La planta es obligatoria.");
        RuleFor(x => x).Must(x => !x.FechaDesde.HasValue || !x.FechaHasta.HasValue
                                  || x.FechaDesde.Value.Date <= x.FechaHasta.Value.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.");
    }
}
