using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Common
{
    /// <summary>
    /// Reglas numéricas comunes a los documentos de venta (Solicitud de Nota de Crédito,
    /// Nota de Crédito, etc.): redondeo, totales con IGV y tipo de cambio por moneda.
    /// </summary>
    public static class DocumentoVentaRules
    {
        /// <summary>Moneda base (ISO 4217): el tipo de cambio de la moneda base es 1.</summary>
        public const string MonedaBase = "PEN";

        public static decimal Round2(decimal value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Sub total = suma de líneas (ya redondeadas a 2 decimales);
        /// IGV = round(subTotal x IGV% / 100, 2); Total = subTotal + IGV.
        /// </summary>
        public static (decimal SubTotal, decimal IgvAmount, decimal TotalAmount) ComputeTotals(
            decimal subTotal, decimal igvPercentage)
        {
            var sub = Round2(subTotal);
            var igv = Round2(sub * igvPercentage / 100m);
            return (sub, igv, sub + igv);
        }

        /// <summary>
        /// Tipo de cambio venta de la moneda en la fecha (Comunes.TipoCambio.SellRate).
        /// Moneda base -> 1. Si la moneda no es la base y no hay tipo de cambio ese día, falla
        /// (mismo criterio del legacy: fg_cambio(fecha,"V") debe ser mayor a cero).
        /// </summary>
        public static async Task<decimal> ResolveExchangeRateAsync(
            IUnitOfWork uow, string monedaCode, DateTime date, CancellationToken cancellationToken)
        {
            if (monedaCode == MonedaBase)
            {
                return 1m;
            }

            var day = DateOnly.FromDateTime(date);

            var rate = await uow.Comunes.TiposCambio.Query()
                .AsNoTracking()
                .Where(t => t.MonedaCode == monedaCode && t.Date == day)
                .Select(t => (decimal?)t.SellRate)
                .FirstOrDefaultAsync(cancellationToken);

            if (!rate.HasValue || rate.Value <= 0)
            {
                throw new InvalidOperationException(
                    $"No se registra el tipo de cambio venta de la moneda {monedaCode} para el {day:dd/MM/yyyy}.");
            }

            return rate.Value;
        }
    }
}
