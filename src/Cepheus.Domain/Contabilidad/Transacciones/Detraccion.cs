using Cepheus.Domain.Comun;
using Cepheus.Domain.Contabilidad.Enum;

namespace Cepheus.Domain.Contabilidad.Transacciones
{
    /// <summary>
    /// Detracción de Adquiriente (depósito masivo). Cabecera del lote.
    ///
    /// Legacy: dbo.COM_DETRACCION (SQL Server).
    /// PK legacy y actual: (Year, Period, LotNumber) = (ANO_DETRACCION, NUM_PERIODO, NUM_LOTE).
    /// El lote es correlativo de 4 dígitos por año y periodo.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   ANO_DETRACCION        -> Year (char(4))
    ///   NUM_PERIODO           -> Period (char(2), mes 01-12)
    ///   NUM_LOTE              -> LotNumber (char(4))
    ///   FEC_INICIO_DETRACCION -> StartDate (solo fecha; mismo año y mes que Year/Period)
    ///   FEC_FIN_DETRACCION    -> EndDate (solo fecha; el legacy guardaba la hora 00:00 y dejaba fuera
    ///                            los documentos procesados el último día)
    ///   FEC_PROCESO           -> CreatedAt (auditoría; el legacy lo llenaba con getdate())
    ///   FEC_DEPOSITO          -> DepositDate (se llena al generar el archivo de depósito masivo)
    ///   NUM_COMP_PAGO         -> PaymentReceiptNumber (comprobante de pago de tesorería)
    ///   FEC_CANCELACION       -> CancellationDate
    ///   IMP_TOT_LOTE          -> TotalSol (suma de DepositSol de los ítems)
    ///   COD_ESTADO            -> Status (enum EstadoDetraccion)
    ///   login_mus             -> CreatedBy (auditoría)
    ///   NUM_ASIENTO           -> AsientoCode (sin FK por ahora)
    ///   Codigo_che            -> ChequeCode (sin FK por ahora)
    /// </summary>
    public class Detraccion : IAuditableEntity
    {
        public string Year { get; set; } = default!;
        public string Period { get; set; } = default!;
        public string LotNumber { get; set; } = default!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public DateTime? DepositDate { get; set; }

        /// <summary>Número del comprobante de pago con que tesorería canceló el lote.</summary>
        public string? PaymentReceiptNumber { get; set; }
        public DateTime? CancellationDate { get; set; }

        public decimal TotalSol { get; set; }

        public EstadoDetraccion Status { get; set; } = EstadoDetraccion.Pendiente;

        /// <summary>
        /// Código del asiento contable del lote (legacy NUM_ASIENTO, char(13)).
        /// TODO: relacionar con AsientoContable cuando se defina el asiento de detracciones.
        /// </summary>
        public string? AsientoCode { get; set; }

        /// <summary>
        /// Código del cheque de tesorería (legacy Codigo_che, char(10)).
        /// TODO: relacionar con la entidad de cheques cuando se suba el módulo de Tesorería.
        /// </summary>
        public string? ChequeCode { get; set; }

        public ICollection<DetraccionItem> Items { get; set; } = new List<DetraccionItem>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
