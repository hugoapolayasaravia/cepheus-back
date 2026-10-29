using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Línea de una programación de Concreto: qué línea de la OT se programa y en qué cantidad. Legacy:
    /// ConcretoAdicional (Codigo_pro, Codigo_Ped, Codigo_Tpr, Codigo_Pru, Clave -> ProgramacionCode + Item de la OT, que
    /// identifica la línea, su producto y su clave).
    ///
    ///   Cantidad_Adi    -> Quantity
    ///   Cantidad_AdiEnt -> DeliveredQuantity (la actualizan las guías: ad_actualizasaldos)
    ///   Codigo_Ufi      -> PumpCode (bomba; solo en la línea de bombeo tipo 04; sin bomba = null, el legacy usa "S/BO")
    ///   Apoyo           -> Support (0 a 9; solo bombeo)
    ///   Cantidad_AdiSal -> NO se migra (siempre 0 en el legacy)
    /// </summary>
    public class ProgramacionConcretoDetalle : IAuditableEntity
    {
        public string ProgramacionCode { get; set; } = default!;
        public ProgramacionConcreto Programacion { get; set; } = default!;

        /// <summary>Item (Clave) de la línea de la OT.</summary>
        public int Item { get; set; }
        public string OrdenTrabajoCode { get; set; } = default!;
        public OrdenTrabajoVentaDetalle OrdenTrabajoDetalle { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal DeliveredQuantity { get; set; }

        public string? PumpCode { get; set; }
        public byte? Support { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
