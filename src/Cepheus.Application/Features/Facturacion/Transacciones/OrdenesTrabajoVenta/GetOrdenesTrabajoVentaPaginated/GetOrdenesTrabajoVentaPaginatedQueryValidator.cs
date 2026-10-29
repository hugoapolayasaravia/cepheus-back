using Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.OrdenesTrabajoVenta.GetOrdenesTrabajoVentaPaginated;

public sealed class GetOrdenesTrabajoVentaPaginatedQueryValidator : AbstractValidator<GetOrdenesTrabajoVentaPaginatedQuery>
{
    public GetOrdenesTrabajoVentaPaginatedQueryValidator()
    {
        RuleFor(x => x.ord).MaximumLength(8);
        RuleFor(x => x.cli).MaximumLength(5);
        RuleFor(x => x.obr).MaximumLength(3);
        RuleFor(x => x.ven).MaximumLength(4);
        RuleFor(x => x.tpr).MaximumLength(2);
        RuleFor(x => x.prd).MaximumLength(4);
        RuleFor(x => x.ot).MaximumLength(8);
        RuleFor(x => x.opt).MaximumLength(1);
        RuleFor(x => x.sis).MaximumLength(1);

        RuleFor(x => x.est)
            .Must(e => OrdenTrabajoVentaRules.IsAll(e) || OrdenTrabajoVentaRules.TryParseEstado(e, out _))
            .WithMessage("est debe ser T o un estado válido: 05 (Activo), 06 (Inactivo), 11 (Cerrado), 42 (Cerrado definitivo).");

        RuleFor(x => x.eta)
            .Must(e => OrdenTrabajoVentaRules.IsAll(e) || OrdenTrabajoVentaRules.TryParseEtapa(e, out _))
            .WithMessage("eta debe ser T o una etapa válida: P, A, C, G.");

        RuleFor(x => x.tip)
            .Must(e => OrdenTrabajoVentaRules.IsAll(e) || OrdenTrabajoVentaRules.TryParseTipo(e, out _))
            .WithMessage("tip debe ser T o un tipo válido: N, S, A, D, C.");

        RuleFor(x => x.vista)
            .Must(v => string.IsNullOrWhiteSpace(v) || new[] { "T", "P", "C" }.Contains(v.Trim().ToUpperInvariant()))
            .WithMessage("vista debe ser T, P o C.");

        RuleFor(x => x)
            .Must(x => !x.fIni.HasValue || x.fFin.HasValue)
            .WithMessage("Si indica fIni debe indicar fFin.");

        RuleFor(x => x)
            .Must(x => !x.fIni.HasValue || !x.fFin.HasValue || x.fIni <= x.fFin)
            .WithMessage("fIni no puede ser mayor a fFin.");
    }
}
