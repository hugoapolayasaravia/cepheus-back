namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la guía de remisión de ventas (RGuiVentas.Estado). Se guarda con el código legacy.
    /// Pendiente (01) es el único estado en el que se puede modificar o anular; Facturado (02) y Cancelado (03) los fijan
    /// los comprobantes de pago (entregable 5).
    /// </summary>
    public enum EstadoGuiaRemisionVenta
    {
        Pendiente = 1,
        Facturado = 2,
        Cancelado = 3,
        Anulado = 4,
        Comprado = 10,
        Procesado = 13,
    }
}
