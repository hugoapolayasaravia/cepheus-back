using Cepheus.Domain.Logistica.Enum;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesServicioSalida.GetOrdenesServicioSalidaPaginated;

public sealed class GetOrdenesServicioSalidaPaginatedQueryValidator
    : AbstractValidator<GetOrdenesServicioSalidaPaginatedQuery>
{
    public GetOrdenesServicioSalidaPaginatedQueryValidator()
    {
        RuleFor(x => x.Codigo_Pla).NotEmpty().MaximumLength(2).WithMessage("Codigo_Pla es obligatorio.");
        RuleFor(x => x.Codigo_Val).MaximumLength(6);
        RuleFor(x => x.Codigo_Tra).MaximumLength(5);

        RuleFor(x => x.Codigo_Est)
            .Must(e => IsAll(e) || TryParseEstado(e, out _))
            .WithMessage("Codigo_Est debe ser 'T' o un estado válido (01, 13, 04).");

        When(x => IsAll(x.Codigo_Val), () =>
        {
            RuleFor(x => x.Fecha_Ini).NotNull().WithMessage("Fecha_Ini es obligatoria cuando Codigo_Val es 'T'.");
            RuleFor(x => x.Fecha_Fin).NotNull().WithMessage("Fecha_Fin es obligatoria cuando Codigo_Val es 'T'.");
            RuleFor(x => x).Must(x => !x.Fecha_Ini.HasValue || !x.Fecha_Fin.HasValue || x.Fecha_Ini <= x.Fecha_Fin)
                .WithMessage("Fecha_Ini no puede ser mayor a Fecha_Fin.");
        });
    }

    internal static bool IsAll(string? value)
        => string.IsNullOrWhiteSpace(value) || value.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);

    internal static bool TryParseEstado(string? value, out EstadoOrdenServicioSalida estado)
    {
        switch (value?.Trim())
        {
            case "01": estado = EstadoOrdenServicioSalida.Pendiente; return true;
            case "13": estado = EstadoOrdenServicioSalida.Procesado; return true;
            case "04": estado = EstadoOrdenServicioSalida.Anulado; return true;
            default:
                return System.Enum.TryParse(value?.Trim(), true, out estado) && System.Enum.IsDefined(estado);
        }
    }
}
