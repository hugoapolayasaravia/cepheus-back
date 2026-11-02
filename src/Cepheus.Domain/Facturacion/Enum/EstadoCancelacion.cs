namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la cancelación de documentos.
    ///
    /// Legacy: dbo.RCanDocumentos.Estado (char(2)):
    ///   01 -> Pendiente: registrada, aún sin asiento.
    ///   02 -> Facturado: con asiento generado y saldo disponible.
    ///   03 -> Cancelado: el importe de la cancelación se aplicó por completo a documentos.
    ///   04 -> Anulado.
    /// </summary>
    public enum EstadoCancelacion
    {
        Pendiente = 1,
        Facturado = 2,
        Cancelado = 3,
        Anulado = 4
    }
}
