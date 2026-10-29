using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Domain.Logistica.Enum;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Reportes.Ajustes.GetReporteAjustesInventario;

public sealed class GetReporteAjustesInventarioQueryValidator : AbstractValidator<GetReporteAjustesInventarioQuery>
{
    public GetReporteAjustesInventarioQueryValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2).WithMessage("La planta es obligatoria.");
        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();
        RuleFor(x => x).Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.");

        RuleFor(x => x.Tipo)
            .Must(t => ReporteRules.EsTodos(t) || Enum.TryParse<TipoAjusteInventario>(t, true, out _))
            .WithMessage("Tipo debe ser T (todos), Sobrante (0) o Faltante (1).");
    }
}
