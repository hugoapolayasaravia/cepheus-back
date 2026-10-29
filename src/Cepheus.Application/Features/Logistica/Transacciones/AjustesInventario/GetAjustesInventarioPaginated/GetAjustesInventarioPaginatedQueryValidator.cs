using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.GetAjustesInventarioPaginated;

public sealed class GetAjustesInventarioPaginatedQueryValidator
    : AbstractValidator<GetAjustesInventarioPaginatedQuery>
{
    public GetAjustesInventarioPaginatedQueryValidator()
    {
        RuleFor(x => x.Codigo_Pla).NotEmpty().MaximumLength(2)
            .WithMessage("Codigo_Pla es obligatorio.");

        RuleFor(x => x.Codigo_Aju).MaximumLength(6);
        RuleFor(x => x.Codigo_Usu).MaximumLength(250);

        RuleFor(x => x.Codigo_Est)
            .Must(e => IsAll(e) || AjusteInventarioRules.TryParseEstado(e, out _))
            .WithMessage("Codigo_Est debe ser 'T' o un estado válido (01, 12, 13, 14, 04).");

        RuleFor(x => x.Tipo_Aju)
            .Must(t => IsAll(t) || AjusteInventarioRules.TryParseTipo(t, out _))
            .WithMessage("Tipo_Aju debe ser 'T', 'I' (sobrante) o 'S' (faltante).");

        When(x => IsAll(x.Codigo_Aju), () =>
        {
            RuleFor(x => x.Fecha_Ini).NotNull()
                .WithMessage("Fecha_Ini es obligatoria cuando Codigo_Aju es 'T'.");
            RuleFor(x => x.Fecha_Fin).NotNull()
                .WithMessage("Fecha_Fin es obligatoria cuando Codigo_Aju es 'T'.");
            RuleFor(x => x)
                .Must(x => !x.Fecha_Ini.HasValue || !x.Fecha_Fin.HasValue || x.Fecha_Ini <= x.Fecha_Fin)
                .WithMessage("Fecha_Ini no puede ser mayor a Fecha_Fin.");
        });
    }

    private static bool IsAll(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);
}
