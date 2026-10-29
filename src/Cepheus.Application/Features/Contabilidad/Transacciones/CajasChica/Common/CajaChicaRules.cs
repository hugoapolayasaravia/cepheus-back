using Cepheus.Domain.Contabilidad.Enum;
using Cepheus.Domain.Contabilidad.Transacciones;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.CajasChica.Common
{
    public static class CajaChicaRules
    {
        public const int MaxLines = 999;
        public const int CodeLength = 6;

        /// <summary>Comprobante "00" = sin documento (no genera documento de compra).</summary>
        public const string ComprobanteSinDocumento = "00";

        public static string Normalize(string value) => value.Trim().ToUpperInvariant();

        public static string TipoNames => string.Join(", ", System.Enum.GetNames(typeof(TipoCajaChica)));

        public static bool TryParseTipo(string? value, out TipoCajaChica tipo)
        {
            tipo = default;
            var text = value?.Trim();

            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            switch (text.ToUpperInvariant())
            {
                case "F": tipo = TipoCajaChica.FondoFijo; return true;
                case "G": tipo = TipoCajaChica.Gastos; return true;
                case "R": tipo = TipoCajaChica.Rendir; return true;
            }

            return System.Enum.TryParse(text, true, out tipo)
                && System.Enum.IsDefined(typeof(TipoCajaChica), tipo);
        }

        public static TipoCajaChica ParseTipo(string value)
            => TryParseTipo(value, out var tipo)
                ? tipo
                : throw new InvalidOperationException($"El tipo de caja debe ser uno de: {TipoNames} (o F, G, R).");

        public static bool TryParseOrigen(string? value, out OrigenLineaCajaChica origen)
        {
            origen = OrigenLineaCajaChica.Manual;
            var text = value?.Trim();

            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            switch (text.ToUpperInvariant())
            {
                case "I": origen = OrigenLineaCajaChica.Manual; return true;
                case "N": origen = OrigenLineaCajaChica.NotaIngreso; return true;
                case "S": origen = OrigenLineaCajaChica.OrdenServicio; return true;
            }

            return System.Enum.TryParse(text, true, out origen)
                && System.Enum.IsDefined(typeof(OrigenLineaCajaChica), origen);
        }

        public static bool HasAsientos(CajaChica caja)
            => !string.IsNullOrWhiteSpace(caja.ProvisionAsientoCode)
               || !string.IsNullOrWhiteSpace(caja.ReversionAsientoCode);

        /// <summary>MontoSol = documentos en moneda base; TotalSol = todos expresados en soles.</summary>
        public static (decimal MontoSol, decimal TotalSol) ComputeTotals(IEnumerable<CajaChicaDetalle> lines, string monedaBase)
        {
            var monto = 0m;
            var total = 0m;

            foreach (var line in lines)
            {
                var esBase = string.Equals(line.MonedaCode, monedaBase, StringComparison.OrdinalIgnoreCase);
                var soles = esBase ? line.Total : Math.Round(line.Total * line.TipoCambio, 2, MidpointRounding.AwayFromZero);

                if (esBase)
                {
                    monto += line.Total;
                }

                total += soles;
            }

            return (monto, total);
        }
    }
}
