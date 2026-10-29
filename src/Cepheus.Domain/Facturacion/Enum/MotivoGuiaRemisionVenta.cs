namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Motivo de traslado de la guía (RGuiVentas.Codigo_mot). Se guarda con el código legacy:
    /// 01 Venta | 02 Traslado entre establecimientos (sin documento de pago) | 10 Devolución | 13 Reposición.
    /// Solo estos cuatro motivos aparecen en las reglas del PB entregado; si existen otros, agregarlos aquí.
    /// </summary>
    public enum MotivoGuiaRemisionVenta
    {
        Venta = 1,
        Traslado = 2,
        Devolucion = 10,
        Reposicion = 13,
    }
}
