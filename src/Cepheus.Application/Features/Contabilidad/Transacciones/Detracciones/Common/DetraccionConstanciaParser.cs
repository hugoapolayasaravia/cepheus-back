using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common
{
    public sealed record ConstanciaRecord(
        string Operacion, string Constancia, string Ruc, string Bien, DateTime? Fecha);

    /// <summary>
    /// Lee el archivo de constancias de depósito que devuelve el banco (legacy: botón "Importa" de la
    /// pestaña Constancia de Depósito Masivo). Cada constancia se identifica por las etiquetas
    /// "Numero de operacion", "Fecha y hora de pago", "Numero de constancia", "RUC Proveedor" y
    /// "Codigo bien o servicio".
    ///
    /// Cambios frente al legacy: no depende de posiciones fijas dentro de la línea, no arrastra datos de
    /// una constancia a la siguiente (se reinician todos los campos) y no depende del orden de las
    /// etiquetas: una constancia termina cuando se repite una etiqueta o termina el archivo.
    /// </summary>
    public static class DetraccionConstanciaParser
    {
        private const string LabelOperacion = "numero de operacion";
        private const string LabelFecha = "fecha y hora de pago";
        private const string LabelConstancia = "numero de constancia";
        private const string LabelRuc = "ruc proveedor";
        private const string LabelBien = "codigo bien o servicio";

        private static readonly Regex RucRegex = new(@"\d{11}", RegexOptions.Compiled);
        private static readonly Regex BienRegex = new(@"\d{3}", RegexOptions.Compiled);
        private static readonly Regex FechaRegex = new(
            @"(\d{1,2})/(\d{1,2})/(\d{4})(?:\s+(\d{1,2}):(\d{2}))?", RegexOptions.Compiled);

        public static List<ConstanciaRecord> Parse(string content)
        {
            var records = new List<ConstanciaRecord>();

            string? operacion = null, constancia = null, ruc = null, bien = null;
            DateTime? fecha = null;
            var hasFecha = false;

            void Flush()
            {
                if (operacion is not null && constancia is not null && ruc is not null && bien is not null)
                {
                    records.Add(new ConstanciaRecord(operacion, constancia, ruc, bien, fecha));
                }

                operacion = constancia = ruc = bien = null;
                fecha = null;
                hasFecha = false;
            }

            foreach (var raw in content.Split('\n'))
            {
                var line = raw.TrimEnd('\r');

                if (line.Trim().Length == 0)
                {
                    continue;
                }

                var normalized = Normalize(line);

                if (TryRest(line, normalized, LabelOperacion, out var rest))
                {
                    var value = FirstToken(rest);

                    if (value is not null)
                    {
                        if (operacion is not null) Flush();
                        operacion = value;
                    }
                }
                else if (TryRest(line, normalized, LabelConstancia, out rest))
                {
                    var value = FirstToken(rest);

                    if (value is not null)
                    {
                        if (constancia is not null) Flush();
                        constancia = value;
                    }
                }
                else if (TryRest(line, normalized, LabelFecha, out rest))
                {
                    var match = FechaRegex.Match(rest);

                    if (match.Success && TryBuildDate(match, out var date))
                    {
                        if (hasFecha) Flush();
                        fecha = date;
                        hasFecha = true;
                    }
                }
                else if (TryRest(line, normalized, LabelRuc, out rest))
                {
                    var match = RucRegex.Match(rest);

                    if (match.Success)
                    {
                        if (ruc is not null) Flush();
                        ruc = match.Value;
                    }
                }
                else if (TryRest(line, normalized, LabelBien, out rest))
                {
                    var match = BienRegex.Match(rest);

                    if (match.Success)
                    {
                        if (bien is not null) Flush();
                        bien = match.Value;
                    }
                }
            }

            Flush();

            return records;
        }

        private static bool TryRest(string line, string normalized, string label, out string rest)
        {
            rest = string.Empty;
            var index = normalized.IndexOf(label, StringComparison.Ordinal);

            if (index < 0)
            {
                return false;
            }

            var start = index + label.Length;
            rest = start >= line.Length ? string.Empty : line[start..].TrimStart(' ', '\t', ':', '-', '.', '=');
            return true;
        }

        private static string? FirstToken(string rest)
        {
            var token = rest.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            return string.IsNullOrWhiteSpace(token) ? null : token.Trim();
        }

        private static bool TryBuildDate(Match match, out DateTime date)
        {
            date = default;

            var day = int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            var month = int.Parse(match.Groups[2].Value, CultureInfo.InvariantCulture);
            var year = int.Parse(match.Groups[3].Value, CultureInfo.InvariantCulture);
            var hour = match.Groups[4].Success ? int.Parse(match.Groups[4].Value, CultureInfo.InvariantCulture) : 0;
            var minute = match.Groups[5].Success ? int.Parse(match.Groups[5].Value, CultureInfo.InvariantCulture) : 0;

            try
            {
                date = new DateTime(year, month, day, hour, minute, 0);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>Minúsculas y sin tildes, conservando el largo para poder ubicar el valor en la línea original.</summary>
        private static string Normalize(string value)
        {
            var decomposed = value.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder(decomposed.Length);

            foreach (var c in decomposed)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    builder.Append(char.ToLowerInvariant(c));
                }
            }

            return builder.ToString();
        }
    }
}
