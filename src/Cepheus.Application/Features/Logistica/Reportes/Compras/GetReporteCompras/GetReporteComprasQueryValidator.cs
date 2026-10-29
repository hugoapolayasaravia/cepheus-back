using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Reportes.Compras.GetReporteCompras;

public sealed class GetReporteComprasQueryValidator : AbstractValidator<GetReporteComprasQuery>
{
    public GetReporteComprasQueryValidator()
    {
        RuleFor(x => x.PlantaCode).NotEmpty().MaximumLength(2).WithMessage("La planta es obligatoria.");
        RuleFor(x => x.FechaDesde).NotEmpty();
        RuleFor(x => x.FechaHasta).NotEmpty();
        RuleFor(x => x).Must(x => x.FechaDesde.Date <= x.FechaHasta.Date)
            .WithMessage("FechaDesde no puede ser mayor a FechaHasta.");

        RuleFor(x => x.Atencion)
            .Must(a => string.IsNullOrWhiteSpace(a) || new[] { "T", "N", "S" }.Contains(a.Trim().ToUpperInvariant()))
            .WithMessage("Atención debe ser T (todos), N (Notas de Ingreso) o S (Órdenes de Servicio).");

        RuleFor(x => x.OrdenarPor)
            .Must(o => string.IsNullOrWhiteSpace(o) || GetReporteComprasQueryHandler.Ordenamientos
                .Contains(o.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("OrdenarPor no es válido.");
    }
}
