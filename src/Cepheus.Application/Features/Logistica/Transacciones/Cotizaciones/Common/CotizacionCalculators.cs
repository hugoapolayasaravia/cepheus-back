// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/Common/CotizacionCalculators.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common
{
    public static class CotizacionCalculators
    {
        public static decimal CalculateLineTotal(decimal cantidad, decimal precio, decimal descuento)
            => Math.Round(cantidad * precio - descuento, 2, MidpointRounding.AwayFromZero);

        /// <summary>
        /// Neto/Igv/Total de la oferta de un proveedor, desde sus líneas +
        /// el IGV% del período vigente (Comunes.PeriodoIgv).
        /// </summary>
        public static async Task RecalculateProveedorAsync(
            IUnitOfWork uow, CotizacionProveedor proveedor, CancellationToken cancellationToken)
        {
            // La cotización no tiene fecha de documento: se usa el porcentaje vigente hoy.
            var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(uow, DateTime.Today, cancellationToken);

            var neto = proveedor.Detalles.Sum(d => d.TotalLinea);
            neto = Math.Round(neto, 2, MidpointRounding.AwayFromZero);
            var igv = Math.Round(neto * igvPercentage / 100m, 2, MidpointRounding.AwayFromZero);

            proveedor.NetoCotizacion = neto;
            proveedor.IgvCotizacion = igv;
            proveedor.TotalCotizacion = neto + igv;
        }

        /// <summary>
        /// Recalcula PedidoDetalle.CantidadCotizada (informativo, sin
        /// bloqueo) para las líneas de Pedido afectadas por cambios en
        /// CotizacionPedidoOrigen.
        /// </summary>
        public static async Task RecalculatePedidoCantidadCotizadaAsync(
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

                var cantidadCotizada = await uow.Logistica.Transacciones.CotizacionPedidoOrigenes.Query()
                    .Where(o => o.PlantaCode == plantaCode
                             && o.PedidoCode == pedidoCode
                             && o.PedidoItemNumber == itemNumber)
                    .SumAsync(o => o.CantidadTomada, cancellationToken);

                pedidoDetalle.CantidadCotizada = cantidadCotizada;
                uow.Logistica.Transacciones.PedidoDetalles.Update(pedidoDetalle);
            }
        }
    }
}