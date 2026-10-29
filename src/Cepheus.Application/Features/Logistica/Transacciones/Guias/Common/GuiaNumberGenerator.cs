// Cepheus.Application/Features/Logistica/Transacciones/Guias/Common/GuiaNumberGenerator.cs
using System.Globalization;
using System.Text.RegularExpressions;

namespace Cepheus.Application.Features.Logistica.Transacciones.Guias.Common
{
    /// <summary>
    /// Genera el siguiente número de guía 'SSS-NNNNNN' a partir del último
    /// emitido por la planta (Planta.GuiaNum). Misma lógica que el legacy:
    /// se conserva la serie y se incrementa el correlativo en 1.
    /// </summary>
    internal static class GuiaNumberGenerator
    {
        private static readonly Regex Format = new(@"^[0-9]{3}-[0-9]{6}$", RegexOptions.Compiled);

        public const int MaxCorrelative = 999_999;

        public static string Next(string plantaCode, string? lastGuiaNumber)
        {
            var current = lastGuiaNumber?.Trim();

            if (string.IsNullOrEmpty(current) || !Format.IsMatch(current))
            {
                throw new InvalidOperationException(
                    $"La numeración de guías de la planta {plantaCode} ('{lastGuiaNumber}') no tiene el formato SSS-NNNNNN. Corrija la serie de la planta.");
            }

            var serie = current.Substring(0, 3);
            var next = int.Parse(current.Substring(4), CultureInfo.InvariantCulture) + 1;

            if (next > MaxCorrelative)
            {
                throw new InvalidOperationException(
                    $"Se agotó el correlativo de la serie {serie} de la planta {plantaCode}. Configure una nueva serie.");
            }

            return $"{serie}-{next.ToString("D6", CultureInfo.InvariantCulture)}";
        }
    }
}
