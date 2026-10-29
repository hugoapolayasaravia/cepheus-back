namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la línea de OT (PedidoVentaDetalle.Estado). Se guarda con la letra legacy: P Pendiente | A Anulado.
    /// </summary>
    public enum EstadoDetalleOrdenTrabajoVenta
    {
        Pendiente = 1,
        Anulado = 2,
    }
}
