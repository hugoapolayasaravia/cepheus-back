using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Bitácora de aprobaciones anuladas. Legacy: programacion.dbo.PRO_APROB_AUDITORIA (se inserta al anular la
    /// aprobación con el motivo COD_MOT_OT_ANULACION). Quien anula queda en CreatedBy/CreatedAt (LOGIN_MUS).
    /// </summary>
    public class OrdenTrabajoVentaAprobacionAnulada : IAuditableEntity
    {
        public int Id { get; set; }

        public string OrdenTrabajoCode { get; set; } = default!;
        public OrdenTrabajoVenta OrdenTrabajo { get; set; } = default!;

        /// <summary>Motivo de anulación (COD_MOT_OT_ANULACION, char(2)).</summary>
        public string Reason { get; set; } = default!;

        /// <summary>Observaciones de la anulación (texto libre).</summary>
        public string? Observations { get; set; }

        public TipoDocumentoAprobacion DocumentType { get; set; }
        public string? Document1Number { get; set; }
        public string? Document2Number { get; set; }
        public string? Plant1Code { get; set; }
        public string? Plant2Code { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public string? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
