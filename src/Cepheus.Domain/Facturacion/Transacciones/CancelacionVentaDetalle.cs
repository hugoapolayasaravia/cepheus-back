using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Documento cancelado por una cancelación (detalle). Los importes de las notas de crédito (07) van con signo
    /// negativo, como el legacy; el saldo del documento usa el valor absoluto.
    ///
    /// Legacy: dbo.DCanDocumentos, PK (Codigo_Can, Item_Can).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Can, Item_Can -> CancelacionCode, Item (char(3))
    ///   Codigo_Pla       -> PlantaCode (planta del documento)
    ///   Codigo_Cli       -> ClienteCode
    ///   Fecha_Can        -> CancelacionDate
    ///   Codigo_tdo       -> TypeCode (01 factura, 03 boleta, 07 nota de crédito, 08 nota de débito, 10 letra, 11 cheque)
    ///   Documento_Can    -> DocumentNumber (char(15); en cheques el número del cheque)
    ///   (nuevo)          -> DocumentBancoCode (solo cheques: el legacy ignora el banco; aquí forma parte de la clave)
    ///   FechaDoc_Can     -> DocumentDate
    ///   moneda_can       -> DocumentCurrencyCode (moneda del documento)
    ///   tipcambio        -> DocumentExchangeRate
    ///   TotNetSol/Dol_can -> NetAmount (total del documento en su moneda)
    ///   TotPagSol/Dol_can -> PaidAmount (aplicado, en la moneda del documento) y PaidSoles (su equivalente en soles)
    ///   TotDifSol_can    -> DifferenceSoles (diferencia de cambio, en soles)
    ///   asiprovision_can -> ProvisionEntryNumber (asiento del documento)
    ///   codigo_obr       -> ObraCode
    ///   CobranzaDudosa, TipCamCdu -> IsDoubtful, DoubtfulExchangeRate
    ///   IND_DETRACCION   -> IsDetraction
    ///   Estado           -> Status (EstadoCancelacion de la línea; se copia de la cabecera)
    ///   FecProceso_Can   -> ELIMINADO: auditoría
    /// </summary>
    public class CancelacionVentaDetalle : IAuditableEntity
    {
        public string CancelacionCode { get; set; } = default!;
        public CancelacionVenta Cancelacion { get; set; } = default!;

        public string Item { get; set; } = default!;

        public string PlantaCode { get; set; } = default!;
        public string ClienteCode { get; set; } = default!;
        public DateTime CancelacionDate { get; set; }

        public string TypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string? DocumentBancoCode { get; set; }
        public DateTime DocumentDate { get; set; }

        public string DocumentCurrencyCode { get; set; } = default!;
        public decimal DocumentExchangeRate { get; set; }

        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal PaidSoles { get; set; }
        public decimal DifferenceSoles { get; set; }

        public string? ProvisionEntryNumber { get; set; }
        public string? ObraCode { get; set; }

        public bool IsDoubtful { get; set; }
        public decimal DoubtfulExchangeRate { get; set; }
        public bool IsDetraction { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
