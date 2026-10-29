using System.Globalization;
using Cepheus.Application.Comun.Interfaces;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common
{
    /// <summary>Línea de depósito: un ítem del lote con el periodo tributario que lleva en el archivo.</summary>
    public sealed record DetraccionArchivoItem(
        string ProveedorRuc,
        string TipoBienCode,
        string AccountNumber,
        string TipoOperacionCode,
        decimal DepositSol,
        string Periodo);

    /// <summary>
    /// Arma las líneas del archivo de depósito masivo (legacy: SP com_genera_detraccion).
    ///
    /// Cabecera (68 caracteres): '*' + RUC adquiriente (11) + razón social (35, relleno con espacios a la
    ///   derecha) + año (2) + lote (4) + total del lote en soles enteros con 2 decimales (15, ceros a la izquierda).
    /// Detalle (57 caracteres): RUC proveedor (11) + 9 ceros + tipo de bien (3) + cuenta de detracciones (11,
    ///   ceros a la izquierda) + importe con 2 decimales (15, ceros a la izquierda) + tipo de operación (2)
    ///   + periodo AAAAMM (6).
    ///
    /// Cambios frente al legacy: el importe se arma con centavos exactos (el legacy redondeaba el entero con
    /// STR y tomaba los decimales de otra conversión) y los ítems con depósito en 0 no generan línea.
    /// </summary>
    public static class DetraccionArchivoBuilder
    {
        public const int NameLength = 35;
        public const int AmountLength = 15;

        public static List<string> BuildLines(
            string year,
            string lot,
            IReadOnlyList<DetraccionArchivoItem> items,
            IDetraccionSettings settings)
        {
            var ruc = settings.AdquirienteRuc?.Trim() ?? string.Empty;

            if (ruc.Length != 11 || !ruc.All(char.IsDigit))
            {
                throw new InvalidOperationException(
                    "Detraccion:AdquirienteRuc debe tener 11 dígitos. Configúrelo en appsettings.");
            }

            var name = settings.AdquirienteName?.Trim() ?? string.Empty;

            if (name.Length == 0)
            {
                throw new InvalidOperationException(
                    "Falta configurar Detraccion:AdquirienteName (razón social) en appsettings.");
            }

            if (name.Length > NameLength)
            {
                name = name[..NameLength];
            }

            var lines = new List<string>();
            var total = items.Sum(i => i.DepositSol);

            var totalCents = (long)Math.Round(Math.Round(total, 0, MidpointRounding.AwayFromZero) * 100m,
                0, MidpointRounding.AwayFromZero);

            lines.Add(
                "*" + ruc + name.PadRight(NameLength) + year[^2..] + lot + Amount(totalCents));

            foreach (var item in items)
            {
                var cents = (long)Math.Round(item.DepositSol * 100m, 0, MidpointRounding.AwayFromZero);

                lines.Add(
                    Exact(item.ProveedorRuc, 11, "RUC del proveedor")
                    + new string('0', 9)
                    + item.TipoBienCode.Trim().PadLeft(3, '0')
                    + Account(item.AccountNumber)
                    + Amount(cents)
                    + item.TipoOperacionCode.Trim().PadLeft(2, '0')
                    + Exact(item.Periodo, 6, "periodo"));
            }

            return lines;
        }

        private static string Amount(long cents)
        {
            var text = cents.ToString(CultureInfo.InvariantCulture).PadLeft(AmountLength, '0');

            if (text.Length > AmountLength)
            {
                throw new InvalidOperationException("Un importe excede el largo permitido por el archivo de depósito.");
            }

            return text;
        }

        private static string Account(string accountNumber)
        {
            var digits = new string(accountNumber.Where(char.IsDigit).ToArray());

            if (digits.Length == 0)
            {
                throw new InvalidOperationException("Un ítem no tiene número de cuenta de detracciones.");
            }

            return digits.Length >= 11 ? digits[^11..] : digits.PadLeft(11, '0');
        }

        private static string Exact(string value, int length, string label)
        {
            var text = value.Trim();

            if (text.Length != length)
            {
                throw new InvalidOperationException($"El {label} '{text}' debe tener {length} caracteres.");
            }

            return text;
        }
    }
}
