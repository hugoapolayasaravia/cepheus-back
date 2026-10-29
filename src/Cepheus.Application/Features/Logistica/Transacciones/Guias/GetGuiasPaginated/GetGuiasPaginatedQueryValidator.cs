// Cepheus.Application/Features/Logistica/Transacciones/Guias/GetGuiasPaginated/GetGuiasPaginatedQueryValidator.cs
using Cepheus.Domain.Logistica.Enum;
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.GetGuiasPaginated
{
    public class GetGuiasPaginatedQueryValidator : AbstractValidator<GetGuiasPaginatedQuery>
    {
        public GetGuiasPaginatedQueryValidator()
        {
            RuleFor(x => x.Estado)
                .Must(IsValidEstadoFilter)
                .WithMessage("El estado indicado no es válido. Valores permitidos: T, Pendiente, Anulado.")
                .When(x => !string.IsNullOrWhiteSpace(x.Estado));

            RuleFor(x => x.Serie)
                .Matches("^[0-9]{3}$")
                .WithMessage("La serie debe tener 3 dígitos (o T para todas).")
                .When(x => !string.IsNullOrWhiteSpace(x.Serie)
                        && !x.Serie.Trim().Equals("T", StringComparison.OrdinalIgnoreCase));

            RuleFor(x => x)
                .Must(x => x.FechaInicio!.Value.Date <= x.FechaFin!.Value.Date)
                .WithMessage("La fecha inicial no puede ser mayor a la fecha final.")
                .When(x => x.FechaInicio.HasValue && x.FechaFin.HasValue);
        }

        private static bool IsValidEstadoFilter(string? value)
        {
            var text = value!.Trim();

            return text.Equals("T", StringComparison.OrdinalIgnoreCase)
                || (System.Enum.TryParse<EstadoGuia>(text, true, out var estado)
                    && System.Enum.IsDefined(typeof(EstadoGuia), estado));
        }
    }
}
