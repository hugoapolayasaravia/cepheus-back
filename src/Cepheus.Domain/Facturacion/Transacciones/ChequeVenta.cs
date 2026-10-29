using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Cheque de un cliente recibido en una cancelación (tipo de pago 02 cheque / 07 cheque diferido).
    /// El cheque NO se registra a mano: lo crea el módulo de Cancelaciones; este módulo lo consulta, lo rebota,
    /// liquida (cheque diferido) y deshace esas operaciones.
    ///
    /// Legacy: dbo.RCheVentas. PK (Numero_che, Codigo_cli, codigo_ban).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_pla       -> PlantaCode
    ///   Numero_che       -> Number (char(15))
    ///   Codigo_cli       -> ClienteCode
    ///   codigo_ban       -> BancoCode (FK -> Comunes.Banco)
    ///   FecEmision_che   -> IssueDate (fecha de la cancelación)
    ///   Codigo_cob       -> CobradorCode
    ///   FecPago_che      -> PaymentDate (fecha a la que el cheque se puede cobrar; si no es diferido = emisión)
    ///   TipCambio_che    -> ExchangeRate
    ///   moneda_che       -> MonedaCode (S/D -> PEN/USD)
    ///   TotNetoSol_che   -> TotalAmount (una sola moneda + tipo de cambio)
    ///   PendienteSol_che -> PendingAmount (saldo por cobrar: lo mueve Cancelaciones cuando el cheque rebotado se cancela)
    ///   AsiProvision_che -> ProvisionEntryNumber (asiento de la cancelación que lo originó)
    ///   (nuevo)          -> ReboteEntryNumber (asiento de rebote; en el legacy se pisaba AsiProvision_che)
    ///   AsiAnulacion_che -> CancellationEntryNumber, FecAnulacion_che -> CanceledAt
    ///   glosa_che        -> Description
    ///   codigo_can       -> CancelacionCode (cancelación que lo originó)
    ///   operacion_che, fecopera_che -> OperationNumber, OperationDate (liquidación del cheque diferido)
    ///   diferido_che S/N -> IsDeferred
    ///   codigo_mch, fecha_mch -> MotivoReboteCode, ReboteDate
    ///   FecCancelacion_che -> SettledAt
    ///   Estado           -> Status (enum EstadoCheque)
    ///   Clave, FechaFinal_val, impresion_che, FecProceso_che, CobranzaDudosa, TipCamCdu -> no se migran
    /// </summary>
    public class ChequeVenta : IAuditableEntity
    {
        public string Number { get; set; } = default!;

        public string ClienteCode { get; set; } = default!;
        public Cliente Cliente { get; set; } = default!;

        public string BancoCode { get; set; } = default!;
        public Banco Banco { get; set; } = default!;

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public DateTime IssueDate { get; set; }
        public DateTime PaymentDate { get; set; }

        public string? CobradorCode { get; set; }
        public Cobrador? Cobrador { get; set; }

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;
        public decimal ExchangeRate { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal PendingAmount { get; set; }

        public string? ProvisionEntryNumber { get; set; }
        public string? ReboteEntryNumber { get; set; }
        public string? CancellationEntryNumber { get; set; }
        public DateTime? CanceledAt { get; set; }

        public string? Description { get; set; }
        public string CancelacionCode { get; set; } = default!;

        public bool IsDeferred { get; set; }
        public string? OperationNumber { get; set; }
        public DateTime? OperationDate { get; set; }

        public string? MotivoReboteCode { get; set; }
        public MotivoChequeRebote? MotivoRebote { get; set; }
        public DateTime? ReboteDate { get; set; }

        public DateTime? SettledAt { get; set; }

        public EstadoCheque Status { get; set; } = EstadoCheque.Pendiente;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
