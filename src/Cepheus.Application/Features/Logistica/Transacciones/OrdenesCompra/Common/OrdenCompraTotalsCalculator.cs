// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/Common/OrdenCompraTotalsCalculator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common
{
    /// <summary>
    /// Traducción exacta de la fórmula legacy (PowerBuilder, ver análisis
    /// aprobado), generalizada con los flags de ComprobantePago en vez de
    /// un código de documento quemado (confirmado):
    ///
    ///   Suma        = Σ OrdenCompraDetalle.TotalArticulo
    ///   Igv         = ComprobantePago.AffectsIgv        ? round(Suma × IgvPercentage / 100, 2)         : 0
    ///   RentaCompra = ComprobantePago.AffectsIncomeTax   ? round(Suma × WithholdingPercentage / 100, 2) : 0
    ///   FonaviCompra= ComprobantePago.AffectsFonavi      ? round(Suma × FonaviPercentage / 100, 2)      : 0
    ///   Si ComprobantePago.AffectsIncomeTax Y Suma <= WithholdingCap (tope de
    ///     retención SUNAT) -> RentaCompra = 0, FonaviCompra = 0
    ///   NoGravableCompra/ServicioCompra/IgvExteriorCompra: NUNCA se tocan
    ///     acá — son 100% digitados por el usuario (confirmado).
    ///   NetoCompra  = Suma
    ///   TotalCompra = Suma + Igv + NoGravableCompra − RentaCompra − FonaviCompra + ServicioCompra + IgvExteriorCompra
    /// </summary>
    public static class OrdenCompraTotalsCalculator
    {
        public static async Task RecalculateAsync(IUnitOfWork uow, OrdenCompra orden, CancellationToken cancellationToken)
        {
            var suma = Math.Round(orden.Detalles.Sum(d => d.TotalArticulo), 2, MidpointRounding.AwayFromZero);

            var control = await uow.Comunes.ControlesVentas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

            var comprobante = !string.IsNullOrWhiteSpace(orden.ComprobantePagoCode)
                ? await uow.Comunes.ComprobantesPago.Query()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        c => c.Code == orden.ComprobantePagoCode,
                        cancellationToken)
                : null;

            // IGV% del período vigente en la fecha de entrega de la orden (solo si el comprobante afecta IGV).
            var igvPercentage = (comprobante?.AffectsIgv ?? false)
                ? await PeriodoIgvRules.GetPorcentajeAsync(uow, orden.FechaEntrega, cancellationToken)
                : 0m;
            var withholdingPercentage = control?.WithholdingPercentage ?? 0;
            var fonaviPercentage = control?.FonaviPercentage ?? 0;
            var withholdingCap = control?.WithholdingCap ?? 0;

            var igv = (comprobante?.AffectsIgv ?? false)
                ? Math.Round(suma * igvPercentage / 100m, 2, MidpointRounding.AwayFromZero)
                : 0;

            var afectaRenta = comprobante?.AffectsIncomeTax ?? false;
            var rentaSol = afectaRenta
                ? Math.Round(suma * withholdingPercentage / 100m, 2, MidpointRounding.AwayFromZero)
                : 0;
            var fonaviSol = (comprobante?.AffectsFonavi ?? false)
                ? Math.Round(suma * fonaviPercentage / 100m, 2, MidpointRounding.AwayFromZero)
                : 0;

            if (afectaRenta && suma <= withholdingCap)
            {
                rentaSol = 0;
                fonaviSol = 0;
            }

            orden.NetoCompra = suma;
            orden.IgvCompra = igv;
            orden.RentaCompra = rentaSol;
            orden.FonaviCompra = fonaviSol;
            // NoGravableCompra / ServicioCompra / IgvExteriorCompra: sin cambios (manuales).
            orden.TotalCompra = suma + igv + orden.NoGravableCompra - rentaSol - fonaviSol
                                 + orden.ServicioCompra + orden.IgvExteriorCompra;
        }

        public static decimal CalculateLineTotal(decimal cantidad, decimal precio, decimal descuento)
            => Math.Round(cantidad * precio - descuento, 2, MidpointRounding.AwayFromZero);

        /// <summary>Mismo patrón que CotizacionCalculators.RecalculatePedidoCantidadCotizadaAsync, para CantidadEnCompra.</summary>
        public static async Task RecalculatePedidoCantidadEnCompraAsync(
            IUnitOfWork uow,
            IEnumerable<(string PlantaCode, string PedidoCode, int ItemNumber)> pedidoLineas,
            CancellationToken cancellationToken)
        {
            foreach (var (plantaCode, pedidoCode, itemNumber) in pedidoLineas.Distinct())
            {
                var pedidoDetalle = await uow.Logistica.Transacciones.PedidoDetalles.Query()
                    .FirstOrDefaultAsync(pd => pd.PlantaCode == plantaCode
                                             && pd.PedidoCode == pedidoCode
                                             && pd.ItemNumber == itemNumber, cancellationToken);
                if (pedidoDetalle is null) continue;

                var cantidad = await uow.Logistica.Transacciones.OrdenCompraPedidoOrigenes.Query()
                    .Where(o => o.PlantaCode == plantaCode && o.PedidoCode == pedidoCode && o.PedidoItemNumber == itemNumber)
                    .SumAsync(o => o.CantidadTomada, cancellationToken);

                pedidoDetalle.CantidadEnCompra = cantidad;
                uow.Logistica.Transacciones.PedidoDetalles.Update(pedidoDetalle);
            }
        }
    }
}