using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Documento de compra cancelado en un Pago a Proveedor. Detalle.
    ///
    /// Legacy: dbo.DCheCompras (SQL Server). Todo en soles.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Clave                -> Id (identity)
    ///   Codigo_che           -> PagoCode (FK PagoProveedor)
    ///   Codigo_tdo + Numero_dco + Codigo_prv -> ComprobantePagoCode + NumeroDocumento + ProveedorCode
    ///                           (FK compuesta a Contabilidad.DocumentoCompra)
    ///   TotalS_dco           -> TotalSol (total del documento al momento del pago)
    ///   PendienteS_dco       -> PendienteSol (pendiente del documento al momento del pago)
    ///   PagoSol_che          -> PagoSol (monto pagado al documento)
    ///   Asiprovision_dco     -> AsientoProvisionCode (copia del asiento de provisión del documento)
    ///   Detraccion_dco       -> Detraccion (S/N -> bool)
    ///   MonRetencion_che     -> BaseRetencionSol (el proceso guarda aquí el monto afecto a retención)
    ///   AfeRetencion_che     -> MontoRetenidoSol (el proceso guarda aquí la retención calculada)
    ///
    /// No se migran (redundantes o de dólares): numcheque_che, codigo_ban, Moneda_che,
    /// FecEmision_dco y FecPago_dco (se leen del DocumentoCompra: FechaEmision y FechaVencimiento),
    /// tipcambio_tdo, totdifsol_che.
    /// </summary>
    public class PagoProveedorDetalle
    {
        public int Id { get; set; }

        public string PagoCode { get; set; } = default!;
        public PagoProveedor Pago { get; set; } = default!;

        public string ComprobantePagoCode { get; set; } = default!;
        public string NumeroDocumento { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public DocumentoCompra DocumentoCompra { get; set; } = default!;

        public decimal TotalSol { get; set; }
        public decimal PendienteSol { get; set; }
        public decimal PagoSol { get; set; }

        public string? AsientoProvisionCode { get; set; }

        public bool Detraccion { get; set; }

        public decimal BaseRetencionSol { get; set; }
        public decimal MontoRetenidoSol { get; set; }
    }
}
