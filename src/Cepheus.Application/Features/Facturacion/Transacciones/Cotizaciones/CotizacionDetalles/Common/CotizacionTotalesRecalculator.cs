using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.Common
{
    /// <summary>
    /// Recalcula GrossAmount / IgvAmount / NetAmount de la cabecera a partir
    /// del detalle vigente. Se invoca después de crear/editar/eliminar una
    /// línea o de editar la cabecera.
    ///
    ///   Gross = SUM(round(Quantity * UnitPrice, 2))   (legacy: "sub total del detalle")
    ///   Igv   = AppliesIgv ? round((Gross - Discount) * IgvRate / 100, 2) : 0
    ///   Net   = Gross - Discount + Igv   (legacy: Bruto + Igv)
    ///
    /// PENDIENTE CONFIRMAR: el legacy no documenta si Descuento_cot es un
    /// monto o un porcentaje, ni si se resta antes de IGV. Se asume MONTO
    /// restado de la base antes del IGV; ajustar aquí (único punto) si la
    /// regla real es otra.
    /// </summary>
    internal static class CotizacionTotalesRecalculator
    {
        public static async Task RecalculateAsync(
            IUnitOfWork uow, string negocioCode, string year, string month, string code,
            CancellationToken cancellationToken)
        {
            var cotizacion = await uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstAsync(c => c.NegocioCode == negocioCode && c.Year == year
                              && c.Month == month && c.Code == code, cancellationToken);

            var lines = await uow.Facturacion.Transacciones.CotizacionesDetalle.Query()
                .AsNoTracking()
                .Where(d => d.NegocioCode == negocioCode && d.Year == year
                         && d.Month == month && d.Code == code)
                .Select(d => new { d.Quantity, d.UnitPrice })
                .ToListAsync(cancellationToken);

            var gross = lines.Sum(l => Math.Round(l.Quantity * l.UnitPrice, 2));
            var taxable = Math.Max(gross - cotizacion.Discount, 0m);
            var igv = cotizacion.AppliesIgv ? Math.Round(taxable * cotizacion.IgvRate / 100m, 2) : 0m;

            cotizacion.GrossAmount = gross;
            cotizacion.IgvAmount = igv;
            cotizacion.NetAmount = taxable + igv;

            await uow.SaveChangesAsync(cancellationToken);
        }
    }
}
