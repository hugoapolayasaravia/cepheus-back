namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Estado de un Pago a Proveedor (cancelación). Legacy: dbo.RCheCompras.Estado_che (char(2)):
    ///   01 Pendiente, 04 Anulado, 13 Procesado.
    /// El valor numérico coincide con el código legacy.
    ///
    /// Nace Pendiente. Pasa a Procesado cuando se registra el comprobante de retención
    /// (proceso legacy) y a Anulado cuando se anula la operación.
    /// </summary>
    public enum EstadoPagoProveedor
    {
        Pendiente = 1,
        Anulado = 4,
        Procesado = 13
    }
}
