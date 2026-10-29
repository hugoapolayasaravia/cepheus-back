using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using Cepheus.Application.Features.Logistica.Transacciones.ReporteCierre.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.ReporteCierre.GetReporteCierre;

public sealed class GetReporteCierreQueryValidator : AbstractValidator<GetReporteCierreQuery>
{
    public GetReporteCierreQueryValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2)
            .WithMessage("La planta es obligatoria.");

        RuleFor(x => x.PeriodoCode)
            .Must(p => ControlCierreRules.IsValidPeriod(p))
            .WithMessage("El período debe tener el formato AAAAMM (ej. 202609).");

        RuleFor(x => x.PeriodoComparacionCode)
            .Must(p => ControlCierreRules.IsValidPeriod(p))
            .When(x => !string.IsNullOrWhiteSpace(x.PeriodoComparacionCode))
            .WithMessage("El período de comparación debe tener el formato AAAAMM.");

        RuleFor(x => x.Origen)
            .Must(o => ArticuloOrigenRules.EsValido(o))
            .When(x => !string.IsNullOrWhiteSpace(x.Origen))
            .WithMessage("El origen debe ser T (todos), N (nacional) o I (importado).");

        RuleFor(x => x)
            .Must(x => string.IsNullOrWhiteSpace(x.FamiliaHasta) || !string.IsNullOrWhiteSpace(x.FamiliaDesde))
            .WithMessage("Indique la familia inicial del rango.");

        RuleFor(x => x)
            .Must(x => string.IsNullOrWhiteSpace(x.SubFamiliaHasta) || !string.IsNullOrWhiteSpace(x.SubFamiliaDesde))
            .WithMessage("Indique la subfamilia inicial del rango.");
    }
}
