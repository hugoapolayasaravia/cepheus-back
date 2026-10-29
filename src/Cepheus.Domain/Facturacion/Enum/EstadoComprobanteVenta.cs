namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado del comprobante de pago de ventas (RFacVentas.Estado / RBolVentas.Estado), con el código legacy:
    /// 01 Pendiente | 02 Facturado | 03 Cancelado | 04 Anulado.
    /// Este módulo solo crea Pendiente y Anulado; Facturado (provisión contable) y Cancelado (cobranza) los fijan
    /// los módulos de Contabilidad y Cobranzas.
    /// </summary>
    public enum EstadoComprobanteVenta
    {
        Pendiente = 1,
        Facturado = 2,
        Cancelado = 3,
        Anulado = 4,
    }
}
