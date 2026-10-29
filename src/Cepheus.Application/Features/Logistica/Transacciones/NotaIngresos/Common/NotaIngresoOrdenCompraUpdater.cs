using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;

/// <summary>
/// Actualiza lo entregado en la Orden de Compra cuando una Nota de Ingreso se registra o se anula
/// (Cantidad_Ent del legacy), a nivel de línea de OC y de cada pedido de origen:
///   OC 1: pedido 1 art 1 cant 10 + pedido 2 art 1 cant 5  -> NI 1 recibe 10 y 5 por pedido.
///   OC 2: art 1 cant 20 (directa)                          -> NI 2 recibe 20 sobre la línea.
/// No ejecuta SaveChanges.
/// </summary>
public static class NotaIngresoOrdenCompraUpdater
{
    /// <summary>Estados de OC que admiten recepción (el PB bloquea 01 Pendiente, 04 Anulado y 11 Cerrado).</summary>
    public static void EnsureReceivable(OrdenCompra oc)
    {
        if (oc.Estado is not (EstadoOrdenCompra.Aprobado or EstadoOrdenCompra.EntregaParcial))
            throw new InvalidOperationException(
                $"La Orden de Compra {oc.Code} no está disponible para recepción. Estado: {oc.Estado}.");

        if (oc.TipoCompraCode == NotaIngresoRules.TipoCompraServicio)
            throw new InvalidOperationException(
                $"La Orden de Compra {oc.Code} es de tipo Servicio y no ingresa por Nota de Ingreso.");
    }

    /// <summary>Cantidad pendiente de recibir para la línea (y pedido de origen, si lo tiene).</summary>
    public static decimal Pending(OrdenCompraDetalle detalle, string? pedidoCode)
    {
        var lineaPendiente = detalle.CantidadArticulo - detalle.CantidadEntregada;

        if (detalle.Origenes.Count == 0)
            return lineaPendiente;

        var origenPendiente = detalle.Origenes
            .Where(o => o.PedidoCode == pedidoCode)
            .Sum(o => o.CantidadTomada - o.CantidadEntregada);

        return Math.Min(lineaPendiente, origenPendiente);
    }

    public static void ApplyDelivery(OrdenCompraDetalle detalle, string? pedidoCode, decimal cantidad)
    {
        var pending = Pending(detalle, pedidoCode);
        if (cantidad > pending)
            throw new InvalidOperationException(
                $"La cantidad del artículo {detalle.ArticuloCode}{PedidoSuffix(pedidoCode)} ({cantidad}) " +
                $"supera lo pendiente de la Orden de Compra ({pending}).");

        var remaining = cantidad;
        foreach (var origen in detalle.Origenes
                     .Where(o => o.PedidoCode == pedidoCode)
                     .OrderBy(o => o.PedidoItemNumber))
        {
            if (remaining <= 0) break;
            var take = Math.Min(remaining, origen.CantidadTomada - origen.CantidadEntregada);
            if (take <= 0) continue;
            origen.CantidadEntregada += take;
            remaining -= take;
        }

        detalle.CantidadEntregada += cantidad;
    }

    public static void RevertDelivery(OrdenCompraDetalle detalle, string? pedidoCode, decimal cantidad)
    {
        var remaining = cantidad;
        foreach (var origen in detalle.Origenes
                     .Where(o => o.PedidoCode == pedidoCode)
                     .OrderByDescending(o => o.PedidoItemNumber))
        {
            if (remaining <= 0) break;
            var take = Math.Min(remaining, origen.CantidadEntregada);
            if (take <= 0) continue;
            origen.CantidadEntregada -= take;
            remaining -= take;
        }

        detalle.CantidadEntregada = Math.Max(0, detalle.CantidadEntregada - cantidad);
    }

    /// <summary>
    /// Estado de la OC según lo entregado: todo entregado -> Cerrado; algo entregado -> Entrega Parcial;
    /// nada entregado -> Aprobado.
    /// </summary>
    public static void RecalculateEstado(OrdenCompra oc)
    {
        if (oc.Detalles.Count == 0) return;

        if (oc.Detalles.All(d => d.CantidadEntregada >= d.CantidadArticulo))
            oc.Estado = EstadoOrdenCompra.Cerrado;
        else if (oc.Detalles.Any(d => d.CantidadEntregada > 0))
            oc.Estado = EstadoOrdenCompra.EntregaParcial;
        else
            oc.Estado = EstadoOrdenCompra.Aprobado;
    }

    private static string PedidoSuffix(string? pedidoCode)
        => string.IsNullOrEmpty(pedidoCode) ? string.Empty : $" (pedido {pedidoCode})";
}
