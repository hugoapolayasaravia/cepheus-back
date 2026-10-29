using Cepheus.Application.Features.Mantenimiento.Reportes.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.HorometroCalendario.GetReporteHorometroCalendario;

public sealed class GetReporteHorometroCalendarioQueryValidator : AbstractValidator<GetReporteHorometroCalendarioQuery>
{
    public GetReporteHorometroCalendarioQueryValidator()
    {
        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();

        RuleFor(x => x).Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.")
            .OverridePropertyName(nameof(GetReporteHorometroCalendarioQuery.FechaDesde));

        RuleFor(x => x)
            .Must(x => (x.FechaHasta.Date - x.FechaDesde.Date).TotalDays < ReporteRules.MaxDiasCalendario)
            .WithMessage($"El rango no puede exceder {ReporteRules.MaxDiasCalendario} días.")
            .OverridePropertyName(nameof(GetReporteHorometroCalendarioQuery.FechaHasta))
            .When(x => x.FechaDesde.Date <= x.FechaHasta.Date);
    }
}
