// Cepheus.Application/Features/Logistica/Transacciones/Guias/Common/GuiaValidationRules.cs
using FluentValidation;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.Common
{
    internal static class GuiaValidationRules
    {
        public const string HoraPattern = @"^([01][0-9]|2[0-3]):[0-5][0-9]$";

        /// <summary>Cantidad &gt; 0, máx. 2 decimales y dentro de decimal(12,2).</summary>
        public static IRuleBuilderOptions<T, decimal> ValidCantidad<T>(this IRuleBuilder<T, decimal> rule)
            => rule
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0.")
                .LessThan(10_000_000_000m).WithMessage("La cantidad excede el máximo permitido.")
                .Must(c => decimal.Round(c, 2) == c).WithMessage("La cantidad admite como máximo 2 decimales.");
    }
}
