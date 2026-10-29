namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la Nota de Crédito de Ventas.
    ///
    /// Legacy: dbo.RCreVentas.Estado (char(2)):
    ///   01 -> Pendiente (registrada, aún no emitida/impresa)
    ///   02 -> Facturado (emitida)
    ///   03 -> Cancelado
    ///   04 -> Anulado
    /// </summary>
    public enum EstadoNotaCredito
    {
        Pendiente = 1,
        Facturado = 2,
        Cancelado = 3,
        Anulado = 4
    }
}
