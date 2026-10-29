using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.GetValesPaginated;

public sealed class GetValesPaginatedQueryValidator : AbstractValidator<GetValesPaginatedQuery>
{
    public GetValesPaginatedQueryValidator()
    {
        RuleFor(x => x.Codigo_Pla).NotEmpty().MaximumLength(2)
            .WithMessage("Codigo_Pla es obligatorio.");

        RuleFor(x => x.Codigo_Val).MaximumLength(7);
        RuleFor(x => x.Codigo_Usu).MaximumLength(250);
        RuleFor(x => x.Responsable).MaximumLength(5);

        RuleFor(x => x.Codigo_Est)
            .Must(e => IsAll(e) || ValeRules.TryParseEstado(e, out _))
            .WithMessage("Codigo_Est debe ser 'T' o un estado válido (01, 09, 12, 13, 14, 30, 04).");

        RuleFor(x => x)
            .Must(x => !x.Fecha_Ini.HasValue || x.Fecha_Fin.HasValue)
            .WithMessage("Fecha_Fin es obligatoria cuando se indica Fecha_Ini.");

        RuleFor(x => x)
            .Must(x => !x.Fecha_Ini.HasValue || !x.Fecha_Fin.HasValue || x.Fecha_Ini <= x.Fecha_Fin)
            .WithMessage("Fecha_Ini no puede ser mayor a Fecha_Fin.");
    }

    private static bool IsAll(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);
}
