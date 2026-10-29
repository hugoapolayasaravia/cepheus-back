using Cepheus.Domain.Comun;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Mantenimiento.Maestros
{
    /// <summary>
    /// Mano de obra (trabajador, tiempo y costo) prevista en un Plan de
    /// Mantenimiento Preventivo. Detalle hijo de PlanMantenimientoPreventivo.
    ///
    /// Legacy: dbo.TMPManoObras (SQL Server), PK compuesta
    /// (Codigo_Pla, Codigo_Mpv, Codigo_Tra); FK al plan con ON DELETE CASCADE.
    ///   Codigo_Tra -> TrabajadorCode (FK -> Rrhh.Trabajador)
    ///   Tiempo_Pro -> Tiempo (decimal(12,5), default 0)
    ///   Costo_tot  -> CostoTotal (decimal(12,5), default 0)
    ///   Fecha_Pro  -> eliminado (auditoría, CreatedAt)
    /// </summary>
    public class PlanMantenimientoPreventivoManoObra : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string PlanCode { get; set; } = default!;
        public PlanMantenimientoPreventivo Plan { get; set; } = default!;

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public decimal Tiempo { get; set; }
        public decimal CostoTotal { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
