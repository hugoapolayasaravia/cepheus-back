using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Línea de una programación de Otros. Legacy: OtrosDetalle (Codigo_otr, Codigo_tpr + Codigo_Pru, Clave -> ProgramacionCode
    /// + Item de la línea de la OT, que identifica producto y clave).
    ///
    ///   cantidad      -> Quantity (programada)
    ///   Cantidad_ent  -> DeliveredQuantity (la actualizan las guías: ad_actualizasaldos)
    ///   Cantidad_apr  -> NO se guarda: es la aprobada de la línea de la OT.
    /// </summary>
    public class ProgramacionOtrosDetalle : IAuditableEntity
    {
        public string ProgramacionCode { get; set; } = default!;
        public ProgramacionOtros Programacion { get; set; } = default!;

        public int Item { get; set; }
        public string OrdenTrabajoCode { get; set; } = default!;
        public OrdenTrabajoVentaDetalle OrdenTrabajoDetalle { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal DeliveredQuantity { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
