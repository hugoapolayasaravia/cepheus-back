namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Documento de cliente pagado con un cheque mixto. Un cheque mixto lo emite Compras/Tesorería (cabecera
    /// legacy RCheCompras) y paga a la vez documentos de proveedor y documentos de venta de un cliente
    /// (<c>ChequeType</c> = "V"). Aquí solo se LEE: Facturación suma estos pagos al saldo aplicado del documento.
    ///
    /// Legacy: dbo.DCheMixtos. Mapeo de columnas legacy -> propiedades:
    ///   Codigo_che -> ChequeCode          tipo_che -> ChequeType ('V' = documento de venta)
    ///   numcheque_che -> ChequeNumber      codigo_ban -> BancoCode
    ///   Codigo_prv -> ProveedorCode        Codigo_cli -> ClienteCode
    ///   Codigo_tdo -> TypeCode             Numero_dco -> DocumentNumber
    ///   FecEmision_dco -> DocumentDate     FecPago_dco -> DocumentDueDate
    ///   Moneda_che -> CurrencyCode (S/D del legacy se guarda como PEN/USD)
    ///   TotalS_dco -> DocumentTotalSoles   PendienteS_dco -> DocumentPendingSoles
    ///   PagoSol_che -> PaidSoles           PagoRealSol_che -> RealPaidSoles
    ///   tipcambio_tdo -> DocumentExchangeRate   totdifsol_che -> DifferenceSoles
    ///   Asiprovision_dco -> ProvisionEntryNumber   Detraccion_dco -> IsDetraction
    ///   MonRetencion_che -> RetentionAmount   AfeRetencion_che -> RetentionBase
    ///   Clave (identity) -> no se migra; la clave es la PK legacy.
    /// </summary>
    public class ChequeMixtoDocumento
    {
        public string ChequeCode { get; set; } = default!;
        public string ChequeType { get; set; } = default!;
        public string ChequeNumber { get; set; } = default!;
        public string BancoCode { get; set; } = default!;
        public string TypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public string ClienteCode { get; set; } = default!;

        public DateTime DocumentDate { get; set; }
        public DateTime? DocumentDueDate { get; set; }
        public string CurrencyCode { get; set; } = "PEN";

        public decimal DocumentTotalSoles { get; set; }
        public decimal DocumentPendingSoles { get; set; }
        public decimal PaidSoles { get; set; }
        public decimal RealPaidSoles { get; set; }
        public decimal DocumentExchangeRate { get; set; }
        public decimal DifferenceSoles { get; set; }

        public string? ProvisionEntryNumber { get; set; }
        public bool IsDetraction { get; set; }
        public decimal RetentionAmount { get; set; }
        public decimal RetentionBase { get; set; }
    }
}
