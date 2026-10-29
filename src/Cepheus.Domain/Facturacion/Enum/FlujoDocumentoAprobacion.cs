namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Flujo del documento de la aprobación (Aprobaciones.codigo_tip; PB "estado" de la ventana de aprobación).
    /// N = sin documento (crédito); P, G y C = aprobaciones al contado que llevan número de documento y,
    /// con factura, pueden llevar un segundo documento. El significado de cada letra P/G/C no está
    /// documentado en el DDL ni en el PB entregado: se conservan tal cual hasta que se definan sus nombres.
    /// </summary>
    public enum FlujoDocumentoAprobacion
    {
        SinDocumento = 1,
        TipoP = 2,
        TipoG = 3,
        TipoC = 4,
    }
}
