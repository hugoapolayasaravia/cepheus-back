using Cepheus.Application.Features.Logistica.Reportes.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosCentroCostoArticulos.GetReporteConsumosCentroCostoArticulos;

public sealed class GetReporteConsumosCentroCostoArticulosQueryValidator
    : AbstractValidator<GetReporteConsumosCentroCostoArticulosQuery>
{
    public GetReporteConsumosCentroCostoArticulosQueryValidator()
    {
        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();
        RuleFor(x => x).Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.");

        RuleFor(x => x.TipoFecha).Must(TipoFechaConsumo.EsValido)
            .WithMessage("TipoFecha debe ser E (entrega), S (proceso) o A (aprobación).");

        RuleFor(x => x.Fuente)
            .Must(f => ReporteRules.EsTodos(f) || new[] { "N", "S" }.Contains(f!.Trim().ToUpperInvariant()))
            .WithMessage("Fuente debe ser T (todos), N (Vales) o S (Órdenes de Servicio).");

        RuleFor(x => x.ArticuloCode).MaximumLength(7);
    }
}
