namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la Nota de Débito de Ventas.
    ///
    /// Legacy: dbo.RDebVentas.Estado (char(2)):
    ///   01 -> Pendiente (registrada, aún no emitida/impresa)
    ///   02 -> Facturado (emitida, con su asiento de provisión)
    ///   03 -> Cancelado (cobrada por completo mediante cancelaciones)
    ///   04 -> Anulado
    /// </summary>
    public enum EstadoNotaDebito
    {
        Pendiente = 1,
        Facturado = 2,
        Cancelado = 3,
        Anulado = 4
    }
}
