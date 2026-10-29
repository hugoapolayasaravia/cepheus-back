using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Mantenimiento.Transacciones
{
    /// <summary>
    /// Registro de consumo de un Artículo (material/repuesto) en una Orden
    /// de Trabajo. Detalle hijo de OrdenTrabajo — primera integración
    /// directa entre Mantenimiento y el catálogo de Artículos de Logística.
    ///
    /// Legacy: dbo.TOTRMateriales (SQL Server), PK compuesta
    /// (Codigo_Pla, Codigo_Otr, Fecha_Pro, Codigo_Art) — igual criterio de
    /// 4 columnas que OTResponsable (sí incluye fecha, a diferencia de
    /// OTRMaquina).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Pla + Codigo_Otr -> PlantaCode + OrdenTrabajoCode
    ///                              (FK compuesta -> OrdenTrabajo)
    ///   Codigo_Art                -> ArticuloCode (FK ->
    ///                                Logistica.Maestros.Articulo, ya existe)
    ///   Fecha_Pro                 -> FechaProceso (default hoy)
    ///   Cantidad                  -> Cantidad (decimal(12,5), default 0)
    ///   Costo_Uni                 -> CostoUnitario (decimal(12,5), default 0)
    ///   Costo_Tot                 -> CostoTotal (decimal(12,2), default 0)
    ///   Codigo_est                -> EstadoCode (char(2) NULL en el legacy).
    ///                                Sin catálogo ni FK detrás en el script
    ///                                original (ni siquiera aparece en los
    ///                                extended properties con una tabla de
    ///                                referencia) — se conserva como texto
    ///                                libre opcional en vez de inventarle un
    ///                                significado. Si me confirmas qué
    ///                                representa realmente, lo convierto a
    ///                                catálogo o enum.
    ///
    /// Nota de integración (no implementada aún, fuera de este alcance): al
    /// crear un consumo real, lo natural sería descontar stock llamando a
    /// DecreaseArticuloStockCommand de Logística — se deja como
    /// responsabilidad de un caso de uso futuro/orquestador, no del CRUD
    /// básico de este detalle.
    /// </summary>
    public class OTRMaterial : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string OrdenTrabajoCode { get; set; } = default!;
        public OrdenTrabajo OrdenTrabajo { get; set; } = default!;

        public DateTime FechaProceso { get; set; }

        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal CostoTotal { get; set; }

        public string? EstadoCode { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
