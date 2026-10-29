using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Maestros
{
    /// <summary>
    /// Centro Ejecutor: quién ejecuta el trabajo de una Orden de Trabajo
    /// (cuadrilla/área de mantenimiento). No debe confundirse con Centro de
    /// Costo de Logística — ese es dónde se contabiliza el gasto, este es
    /// quién lo realiza. Maestro del módulo Mantenimiento (padre de
    /// SubCentroEjecutor).
    ///
    /// Legacy: dbo.TCentroEjecutor (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_ce        -> Code (PK natural, char(2), código
    ///                       mnemotécnico manual — mismo criterio que
    ///                       Inspeccion.Code)
    ///   Descripcion_ce   -> Name
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos/maestros del sistema.
    /// </summary>
    public class CentroEjecutor : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
