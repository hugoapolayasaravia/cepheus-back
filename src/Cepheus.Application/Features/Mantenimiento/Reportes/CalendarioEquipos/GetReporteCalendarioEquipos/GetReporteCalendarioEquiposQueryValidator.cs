using Cepheus.Application.Features.Mantenimiento.Reportes.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.CalendarioEquipos.GetReporteCalendarioEquipos;

public sealed class GetReporteCalendarioEquiposQueryValidator : AbstractValidator<GetReporteCalendarioEquiposQuery>
{
    public GetReporteCalendarioEquiposQueryValidator()
    {
        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();

        RuleFor(x => x).Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.")
            .OverridePropertyName(nameof(GetReporteCalendarioEquiposQuery.FechaDesde));

        RuleFor(x => x)
            .Must(x => (x.FechaHasta.Date - x.FechaDesde.Date).TotalDays < ReporteRules.MaxDiasCalendario)
            .WithMessage($"El rango no puede exceder {ReporteRules.MaxDiasCalendario} días.")
            .OverridePropertyName(nameof(GetReporteCalendarioEquiposQuery.FechaHasta))
            .When(x => x.FechaDesde.Date <= x.FechaHasta.Date);
    }
}
