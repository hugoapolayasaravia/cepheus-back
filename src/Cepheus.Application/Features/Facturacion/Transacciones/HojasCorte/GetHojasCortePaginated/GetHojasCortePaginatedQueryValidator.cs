using Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Facturacion.Transacciones.HojasCorte.GetHojasCortePaginated;

public sealed class GetHojasCortePaginatedQueryValidator : AbstractValidator<GetHojasCortePaginatedQuery>
{
    public GetHojasCortePaginatedQueryValidator()
    {
        RuleFor(x => x.p_anio).MaximumLength(4);
        RuleFor(x => x.p_nrohco).MaximumLength(8);
        RuleFor(x => x.p_codcli).MaximumLength(5);
        RuleFor(x => x.p_codobr).MaximumLength(3);
        RuleFor(x => x.p_codven).MaximumLength(4);
        RuleFor(x => x.p_codtec).MaximumLength(5);
        RuleFor(x => x.p_tiphco).MaximumLength(2);
        RuleFor(x => x.p_nomcli).MaximumLength(100);
        RuleFor(x => x.p_nomobr).MaximumLength(100);

        RuleFor(x => x.p_anio).NotEmpty().Length(4)
            .When(x => !string.IsNullOrWhiteSpace(x.p_nrohco) &&
                       HojaCorteRules.NormalizeCode(x.p_nrohco).Length == HojaCorteRules.CodeLength)
            .WithMessage("p_anio es obligatorio (4 dígitos) cuando se indica p_nrohco.");

        RuleFor(x => x.p_codest)
            .Must(e => string.IsNullOrWhiteSpace(e) || HojaCorteRules.TryParseEstado(e, out _))
            .WithMessage("p_codest debe ser un estado válido: 01 (Pendiente), 04 (Anulado), 13 (Procesado).");

        RuleFor(x => x)
            .Must(x => !x.p_fecini.HasValue || !x.p_fecfin.HasValue || x.p_fecini <= x.p_fecfin)
            .WithMessage("p_fecini no puede ser mayor a p_fecfin.");
    }
}
