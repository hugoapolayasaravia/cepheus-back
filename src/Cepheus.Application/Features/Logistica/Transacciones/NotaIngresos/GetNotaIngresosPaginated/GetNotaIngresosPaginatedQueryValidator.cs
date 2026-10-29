using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.GetNotaIngresosPaginated;

public sealed class GetNotaIngresosPaginatedQueryValidator : AbstractValidator<GetNotaIngresosPaginatedQuery>
{
    public GetNotaIngresosPaginatedQueryValidator()
    {
        RuleFor(x => x.Codigo_Pla).NotEmpty().MaximumLength(2)
            .WithMessage("Codigo_Pla es obligatorio.");

        RuleFor(x => x.Codigo_NoI).MaximumLength(6);
        RuleFor(x => x.Codigo_Prv).MaximumLength(5);
        RuleFor(x => x.Pago).MaximumLength(2);

        RuleFor(x => x.Codigo_Est)
            .Must(e => IsAll(e) || NotaIngresoRules.TryParseEstado(e, out _))
            .WithMessage("Codigo_Est debe ser 'T' o un estado válido (01, 13, 11, 04).");

        When(x => IsAll(x.Codigo_NoI), () =>
        {
            RuleFor(x => x.Fecha_Ini).NotNull()
                .WithMessage("Fecha_Ini es obligatoria cuando Codigo_NoI es 'T'.");
            RuleFor(x => x.Fecha_Fin).NotNull()
                .WithMessage("Fecha_Fin es obligatoria cuando Codigo_NoI es 'T'.");
            RuleFor(x => x).Must(x => !x.Fecha_Ini.HasValue || !x.Fecha_Fin.HasValue || x.Fecha_Ini <= x.Fecha_Fin)
                .WithMessage("Fecha_Ini no puede ser mayor a Fecha_Fin.");
        });
    }

    private static bool IsAll(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);
}
