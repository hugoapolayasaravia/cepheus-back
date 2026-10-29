using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.ReporteHojaCorte;

public sealed class ReporteHojaCorteQueryValidator : AbstractValidator<ReporteHojaCorteQuery>
{
    public ReporteHojaCorteQueryValidator()
    {
        RuleFor(x => x.p_opcion)
            .Must(o => string.IsNullOrWhiteSpace(o) || o.Trim() is "1" or "2" or "3")
            .WithMessage("p_opcion debe ser 1 (fecha de la hoja), 2 (despacho viguetas cliente) o 3 (despacho bovedillas cliente).");

        RuleFor(x => x)
            .Must(x => !x.p_fecini.HasValue || !x.p_fecfin.HasValue || x.p_fecini.Value.Date <= x.p_fecfin.Value.Date)
            .WithMessage("La fecha inicial no puede ser mayor que la fecha final.");
    }
}
