using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Mantenimiento.Maestros
{
    /// <summary>
    /// Material (artículo, cantidad y costo) previsto en un Plan de
    /// Mantenimiento Preventivo. Detalle hijo de PlanMantenimientoPreventivo;
    /// al generar la OT preventiva se copia a OTRMaterial y al vale de salida.
    ///
    /// Legacy: dbo.TMPMateriales (SQL Server), PK compuesta
    /// (Codigo_Pla, Codigo_Mpv, Codigo_Art); FK al plan con ON DELETE CASCADE.
    ///   Codigo_Art -> ArticuloCode (FK -> Logistica.Maestros.Articulo)
    ///   Cantidad   -> Cantidad (decimal(12,5), default 0)
    ///   Costo_Uni  -> CostoUnitario (decimal(12,5), default 0)
    ///   Costo_Tot  -> CostoTotal (decimal(12,5), default 0)
    ///   Fecha_Pro  -> eliminado (auditoría, CreatedAt)
    /// </summary>
    public class PlanMantenimientoPreventivoMaterial : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string PlanCode { get; set; } = default!;
        public PlanMantenimientoPreventivo Plan { get; set; } = default!;

        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
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
