using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.TiposCambio.Common
{
    /// <summary>
    /// El tipo de cambio se maneja por moneda: cada fila expresa cuántos soles (moneda base) vale una unidad de
    /// la moneda extranjera en una fecha. La moneda base no tiene tipo de cambio (siempre es 1).
    /// </summary>
    public static class TipoCambioRules
    {
        public const string MonedaBase = "PEN";

        /// <summary>Moneda extranjera de referencia del sistema (la del tipo de cambio que usaba el modelo anterior, que era único).</summary>
        public const string MonedaReferencia = "USD";

        public static string Normalizar(string monedaCode) => monedaCode.Trim().ToUpperInvariant();

        public static bool EsMonedaBase(string? monedaCode)
            => string.Equals(monedaCode?.Trim(), MonedaBase, StringComparison.OrdinalIgnoreCase);

        /// <summary>Tipo de cambio venta de la moneda en la fecha; null si no está registrado.</summary>
        public static async Task<decimal?> GetVentaAsync(
            IUnitOfWork uow, string monedaCode, DateOnly fecha, CancellationToken ct)
        {
            var moneda = Normalizar(monedaCode);

            return await uow.Comunes.TiposCambio.Query()
                .AsNoTracking()
                .Where(t => t.MonedaCode == moneda && t.Date == fecha)
                .Select(t => (decimal?)t.SellRate)
                .FirstOrDefaultAsync(ct);
        }

        public static Task<decimal?> GetVentaAsync(
            IUnitOfWork uow, string monedaCode, DateTime fecha, CancellationToken ct)
            => GetVentaAsync(uow, monedaCode, DateOnly.FromDateTime(fecha), ct);
    }
}
