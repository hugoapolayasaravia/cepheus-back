namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la Solicitud de Nota de Crédito de Ventas.
    ///
    /// Legacy: dbo.RCreVentasSol.Codigo_est (char(2)), subconjunto de dbo.TEstados
    /// (mismos valores que usa RCreVentas.Estado):
    ///   01 -> Pendiente (en gestión / aprobación)
    ///   02 -> Facturado (ya se generó la nota de crédito; el PB la marca al grabar la NC)
    ///   03 -> Cancelado (rechazada en alguna etapa)
    ///   04 -> Anulado
    /// </summary>
    public enum EstadoSolicitudNotaCredito
    {
        Pendiente = 1,
        Facturado = 2,
        Cancelado = 3,
        Anulado = 4
    }
}
