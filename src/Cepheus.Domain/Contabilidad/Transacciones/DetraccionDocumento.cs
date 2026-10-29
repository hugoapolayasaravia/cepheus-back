
namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Documento de compra incluido en un ítem de detracción.
    ///
    /// Legacy: dbo.COM_DETRACCION_DOCUMENTOS (SQL Server).
    /// PK legacy y actual: (Year, Period, LotNumber, ProveedorCode, ItemNumber,
    ///                      ComprobantePagoCode, NumeroDocumento).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tdo     -> ComprobantePagoCode
    ///   Numero_dco     -> NumeroDocumento
    ///   IMP_DETRACCION -> DetraccionSol (total del documento x tasa del tipo de bien, redondeado al sol)
    ///   FLG_EXPORTA    -> IncludeInExport (bool; legacy char(1) '1'/'0', default 1)
    ///
    /// Un documento de compra solo puede estar en un lote (índice único por documento). Al eliminar el
    /// lote o el ítem el documento queda libre para otro lote.
    /// </summary>
    public class DetraccionDocumento
    {
        public string Year { get; set; } = default!;
        public string Period { get; set; } = default!;
        public string LotNumber { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public int ItemNumber { get; set; }
        public DetraccionItem Item { get; set; } = default!;

        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public DocumentoCompra DocumentoCompra { get; set; } = default!;

        public decimal DetraccionSol { get; set; }

        /// <summary>
        /// true = el documento entra al depósito masivo. Si es false, su operación y constancia de
        /// detracción se registran a mano (no vienen del archivo de constancias del banco).
        /// </summary>
        public bool IncludeInExport { get; set; } = true;
    }
}
