using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.Common
{
    /// <summary>
    /// Recalcula LinealMeters/TotalVaults/TotalMeters (y sus copias "Alt",
    /// ver nota en CotizacionMetradoResumen) del nivel a partir de su
    /// detalle vigente. Confirmado contra ue_set_calcula_metrado del código
    /// fuente original:
    ///   LinealMeters = SUM(detalle.TotalMaterial) * nivel.Quantity
    ///   TotalVaults  = SUM(detalle.QuantityB)      * nivel.Quantity
    ///   TotalMeters  = SUM(detalle.Area)           * nivel.Quantity
    ///   TotalVaultsAlt = TotalVaults  (copia; cantidades físicas, no
    ///                     dependen de moneda, ver nota en la entidad)
    ///   TotalMetersAlt = TotalMeters  (copia, ídem)
    ///
    /// NO recalcula TotalPrice/PricePerM2 (ni sus "Alt"): esas dependen del
    /// DataWindow dw_lst_metrodo_nivel, cuyo SQL no se ha confirmado
    /// todavía — quedan como las ingresa quien llama, igual que antes.
    /// </summary>
    internal static class CotizacionMetradoResumenRecalculator
    {
        public static async Task RecalculateAsync(
            IUnitOfWork uow, string negocioCode, string year, string month, string code, int levelNumber,
            CancellationToken cancellationToken)
        {
            var resumen = await uow.Facturacion.Transacciones.CotizacionesMetradoResumen.Query()
                .FirstAsync(r => r.NegocioCode == negocioCode && r.Year == year && r.Month == month
                              && r.Code == code && r.LevelNumber == levelNumber, cancellationToken);

            var lines = await uow.Facturacion.Transacciones.CotizacionesMetradoDetalle.Query()
                .AsNoTracking()
                .Where(d => d.NegocioCode == negocioCode && d.Year == year && d.Month == month
                         && d.Code == code && d.LevelNumber == levelNumber)
                .Select(d => new { d.TotalMaterial, d.QuantityB, d.Area })
                .ToListAsync(cancellationToken);

            var sumMaterial = lines.Sum(l => l.TotalMaterial);
            var sumVaults = lines.Sum(l => l.QuantityB);
            var sumArea = lines.Sum(l => l.Area);

            resumen.LinealMeters = sumMaterial * resumen.Quantity;
            resumen.TotalVaults = sumVaults * resumen.Quantity;
            resumen.TotalMeters = sumArea * resumen.Quantity;
            resumen.TotalVaultsAlt = resumen.TotalVaults;
            resumen.TotalMetersAlt = resumen.TotalMeters;

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
