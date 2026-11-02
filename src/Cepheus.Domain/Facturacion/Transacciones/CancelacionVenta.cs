using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Cancelación de documentos de venta (cabecera): el cliente paga (efectivo, cheque, letras, transferencia,
    /// canje...) y el pago se aplica a facturas, boletas, notas de débito y de crédito, letras o cheques rebotados.
    ///
    /// Legacy: dbo.RCanDocumentos (PK Codigo_Can char(10), correlativo numérico global).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Can       -> Code
    ///   codigo_pla       -> PlantaCode
    ///   Codigo_Cli       -> ClienteCode
    ///   Fecha_Can        -> Date
    ///   Codigo_Cob       -> CobradorCode
    ///   Moneda_can       -> MonedaCode (S/D -> PEN/USD)
    ///   TipCambio        -> ExchangeRate
    ///   Codigo_tpa       -> TipoPagoCode (FK -> TipoPago)
    ///   Numero_Can       -> PaymentNumber (número del cheque / letra / voucher)
    ///   Numero_cta       -> AccountNumber (cuenta bancaria de depósito)
    ///   BancoDoc_Can     -> BancoDocCode (banco del cheque)
    ///   BancoDep_Can     -> BancoDepCode (banco de depósito)
    ///   fecdiferida_can  -> DeferredDate, diferido_can -> IsDeferred
    ///   operacion_can, fecopera_can -> OperationNumber, OperationDate
    ///   glosa_can        -> Description
    ///   TotPagSol/Dol_can -> TotalAmount (importe de la cancelación en su moneda)
    ///   TotUtiSol/Dol_can -> UsedAmount (importe aplicado a documentos)
    ///   TotDifSol/Dol_can -> DifferenceAmount (diferencia de cambio)
    ///   TotDisSol/Dol_can -> AvailableAmount (saldo no aplicado = Total - (Usado + Diferencia))
    ///   Asiento_Can      -> ProvisionEntryNumber
    ///   FecRecepcion_can -> ReceptionDate
    ///   Cantida_cuo, tip_cuota (I/F), Fecha_1Pag, Cuota_Ini, Interes -> InstallmentCount, InstallmentType,
    ///                       FirstPaymentDate, InitialAmount, Interest (solo Letras Varias)
    ///   Asiento_anu, Login_anu, Fecha_anu -> CancellationEntryNumber, CanceledBy, CanceledAt
    ///   Login_apr, Fecha_apr -> ApprovedBy, ApprovedAt
    ///   Estado           -> Status (enum EstadoCancelacion)
    ///   FecProceso_can, codigo_usu -> ELIMINADOS: auditoría
    /// </summary>
    public class CancelacionVenta : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string ClienteCode { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;

        public DateTime Date { get; set; }

        public string CobradorCode { get; set; } = default!;
        public Cobrador Cobrador { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;
        public decimal ExchangeRate { get; set; }

        public string TipoPagoCode { get; set; } = default!;
        public TipoPago TipoPago { get; set; } = default!;

        public string? PaymentNumber { get; set; }
        public string? AccountNumber { get; set; }

        public string? BancoDocCode { get; set; }
        public Banco? BancoDoc { get; set; }
        public string? BancoDepCode { get; set; }
        public Banco? BancoDep { get; set; }

        public bool IsDeferred { get; set; }
        public DateTime? DeferredDate { get; set; }

        public string? OperationNumber { get; set; }
        public DateTime? OperationDate { get; set; }

        public string? Description { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal UsedAmount { get; set; }
        public decimal DifferenceAmount { get; set; }
        public decimal AvailableAmount { get; set; }

        public string? ProvisionEntryNumber { get; set; }
        public DateTime? ReceptionDate { get; set; }

        public int InstallmentCount { get; set; }
        public string? InstallmentType { get; set; }
        public DateTime? FirstPaymentDate { get; set; }
        public decimal InitialAmount { get; set; }
        public decimal Interest { get; set; }

        public string? CancellationEntryNumber { get; set; }
        public string? CanceledBy { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string? CancelReason { get; set; }

        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public EstadoCancelacion Status { get; set; } = EstadoCancelacion.Pendiente;

        public ICollection<CancelacionVentaDetalle> Details { get; set; } = new List<CancelacionVentaDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
