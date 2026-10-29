namespace Cepheus.Application.Features.Contabilidad.Transacciones.AsientosNotaIngreso.Common
{
    public static class AsientoNotaIngresoRules
    {
        /// <summary>
        /// Código de ComprobantePago de la nota de crédito. En el legacy el programa compara el código del
        /// comprobante con "07"; es la clave (PK) de ComprobantePago, no el código SUNAT.
        /// </summary>
        public const string NotaCreditoComprobanteCode = "07";

        /// <summary>Máximo de caracteres de la descripción de una línea de asiento (nvarchar(50) del legacy).</summary>
        public const int DescripcionLineaMax = 50;

        public static string Truncate(string? value, int max)
        {
            var v = (value ?? string.Empty).Trim();
            return v.Length <= max ? v : v.Substring(0, max);
        }
    }
}
