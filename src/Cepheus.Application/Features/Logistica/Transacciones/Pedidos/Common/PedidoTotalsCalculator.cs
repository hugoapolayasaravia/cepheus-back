// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/Common/PedidoTotalsCalculator.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common
{
    /// <summary>
    /// Recalcula Neto/Igv/Total del Pedido a partir de sus líneas activas
    /// (se excluyen las líneas en Estado Anulado) y del IGV vigente en
    /// Comunes.ControlVentas (fila única de configuración). Se invoca cada
    /// vez que cambia el detalle (crear/actualizar/eliminar línea).
    /// Redondeo: 2 decimales, MidpointRounding.AwayFromZero (confirmado).
    /// </summary>
    public static class PedidoTotalsCalculator
    {
        public static async Task RecalculateAsync(IUnitOfWork uow, Pedido pedido, CancellationToken cancellationToken)
        {
            // IGV% del período vigente en la fecha de entrega del pedido.
            var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(uow, pedido.FechaEntrega, cancellationToken);

            var neto = pedido.Detalles
                .Where(d => d.EstadoPedidoDetalle != EstadoPedidoDetalle.Anulado)
                .Sum(d => d.TotalArticulo);

            neto = Math.Round(neto, 2, MidpointRounding.AwayFromZero);
            var igv = Math.Round(neto * igvPercentage / 100m, 2, MidpointRounding.AwayFromZero);

            pedido.NetoPedido = neto;
            pedido.IgvPedido = igv;
            pedido.TotalPedido = neto + igv;
        }

        public static decimal CalculateLineTotal(decimal precio, decimal cantidad)
            => Math.Round(precio * cantidad, 2, MidpointRounding.AwayFromZero);
    }
}