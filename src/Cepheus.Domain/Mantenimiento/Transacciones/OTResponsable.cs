using Cepheus.Domain.Comun;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Mantenimiento.Transacciones
{
    /// <summary>
    /// Registro de mano de obra (tiempo y costo) de un trabajador en una
    /// fecha determinada, dentro de una Orden de Trabajo. Detalle hijo de
    /// OrdenTrabajo.
    ///
    /// Legacy: dbo.TOTResponsables (SQL Server), PK compuesta
    /// (Codigo_Pla, Codigo_Otr, Fecha_Pro, Codigo_Tra) — se mantiene la PK
    /// compuesta de 4 columnas tal cual.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Pla + Codigo_Otr -> PlantaCode + OrdenTrabajoCode
    ///                              (FK compuesta -> OrdenTrabajo)
    ///   Fecha_Pro                -> FechaProceso (default hoy, igual legacy)
    ///   Codigo_Tra                -> TrabajadorCode (SIN FK real: Trabajador
    ///                                aún no existe como tabla, igual
    ///                                criterio que OrdenTrabajo.ResponsableCode)
    ///   T_Proceso                 -> TiempoProceso (horas trabajadas)
    ///   Basico                    -> Basico (monto básico)
    ///   Costo_tot                 -> CostoTotal
    /// </summary>
    public class OTResponsable : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string OrdenTrabajoCode { get; set; } = default!;
        public OrdenTrabajo OrdenTrabajo { get; set; } = default!;

        public DateTime FechaProceso { get; set; }

        // Sin FK real todavía: Trabajador aún no existe como tabla
        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Responsable { get; set; } = default!;

        public decimal TiempoProceso { get; set; }
        public decimal Basico { get; set; }
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
