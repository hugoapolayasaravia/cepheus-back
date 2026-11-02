using System.Text.RegularExpressions;

namespace Cepheus.Application.Features.Comunes.Plantas.Common
{
    /// <summary>
    /// Numeración de documentos de la planta: guarda el ÚLTIMO número emitido con formato "SSS-NNNNNN"
    /// (serie de 3 + guion + correlativo de 6), como Numero_fve char(10) del sistema anterior.
    /// </summary>
    public static class PlantaNumeracion
    {
        public const string Inicial = "001-000000";

        private static readonly Regex Formato = new(@"^\d{3}-\d{6}$", RegexOptions.Compiled);

        /// <summary>Vacío (no se modifica) o con el formato SSS-NNNNNN.</summary>
        public static bool EsValidoOVacio(string? numero)
            => string.IsNullOrWhiteSpace(numero) || Formato.IsMatch(numero.Trim());

        public const string Mensaje = "La numeración debe tener el formato SSS-NNNNNN (por ejemplo 001-000125).";
    }
}
