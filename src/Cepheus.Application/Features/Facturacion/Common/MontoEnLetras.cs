namespace Cepheus.Application.Features.Facturacion.Common
{
    /// <summary>
    /// Monto en letras para documentos impresos. Ejemplo: 1250.50 PEN -> "MIL DOSCIENTOS CINCUENTA CON 50/100 SOLES".
    /// </summary>
    public static class MontoEnLetras
    {
        private static readonly string[] Unidades =
        {
            "CERO", "UNO", "DOS", "TRES", "CUATRO", "CINCO", "SEIS", "SIETE", "OCHO", "NUEVE", "DIEZ",
            "ONCE", "DOCE", "TRECE", "CATORCE", "QUINCE", "DIECISEIS", "DIECISIETE", "DIECIOCHO", "DIECINUEVE",
            "VEINTE", "VEINTIUNO", "VEINTIDOS", "VEINTITRES", "VEINTICUATRO", "VEINTICINCO", "VEINTISEIS",
            "VEINTISIETE", "VEINTIOCHO", "VEINTINUEVE"
        };

        private static readonly string[] Decenas =
            { "", "", "", "TREINTA", "CUARENTA", "CINCUENTA", "SESENTA", "SETENTA", "OCHENTA", "NOVENTA" };

        private static readonly string[] Centenas =
        {
            "", "CIENTO", "DOSCIENTOS", "TRESCIENTOS", "CUATROCIENTOS", "QUINIENTOS", "SEISCIENTOS",
            "SETECIENTOS", "OCHOCIENTOS", "NOVECIENTOS"
        };

        public static string ToWords(decimal amount, string currencyCode)
        {
            var rounded = Math.Round(Math.Abs(amount), 2, MidpointRounding.AwayFromZero);
            var integer = (long)Math.Truncate(rounded);
            var cents = (int)Math.Round((rounded - integer) * 100m, 0, MidpointRounding.AwayFromZero);

            var currency = (currencyCode ?? string.Empty).Trim().ToUpperInvariant() switch
            {
                "PEN" => "SOLES",
                "USD" => "DOLARES AMERICANOS",
                "EUR" => "EUROS",
                var other => other
            };

            return $"{Integer(integer)} CON {cents:00}/100 {currency}".Trim();
        }

        private static string Integer(long n)
        {
            if (n == 0) return "CERO";
            if (n >= 1_000_000_000_000) throw new ArgumentOutOfRangeException(nameof(n));

            var parts = new List<string>();

            var millions = n / 1_000_000;
            if (millions > 0)
            {
                parts.Add(millions == 1 ? "UN MILLON" : $"{Below1000(millions, apocope: true)} MILLONES");
                n %= 1_000_000;
            }

            var thousands = n / 1_000;
            if (thousands > 0)
            {
                parts.Add(thousands == 1 ? "MIL" : $"{Below1000(thousands, apocope: true)} MIL");
                n %= 1_000;
            }

            if (n > 0) parts.Add(Below1000(n, apocope: false));

            return string.Join(" ", parts);
        }

        // apocope: "UNO" -> "UN" cuando precede a MIL / MILLONES (21 -> VEINTIUN MIL).
        private static string Below1000(long n, bool apocope)
        {
            if (n == 100) return "CIEN";

            var words = new List<string>();
            var hundreds = (int)(n / 100);
            var rest = (int)(n % 100);

            if (hundreds > 0) words.Add(Centenas[hundreds]);

            if (rest > 0)
            {
                string tail;
                if (rest < 30)
                {
                    tail = Unidades[rest];
                }
                else
                {
                    var t = rest / 10;
                    var u = rest % 10;
                    tail = u == 0 ? Decenas[t] : $"{Decenas[t]} Y {Unidades[u]}";
                }

                if (apocope)
                {
                    if (tail.EndsWith("VEINTIUNO")) tail = tail[..^1];
                    else if (tail.EndsWith("UNO")) tail = tail[..^1];
                }

                words.Add(tail);
            }

            return string.Join(" ", words);
        }
    }
}
