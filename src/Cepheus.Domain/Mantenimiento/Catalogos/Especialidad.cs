using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Catalogos
{
    /// <summary>
    /// Especialidad técnica de una Orden de Trabajo (ej. "M" = Mecánica).
    /// Catálogo del módulo Mantenimiento.
    ///
    /// Legacy: dbo.TEspecialidades (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Esp       -> Code (PK natural, char(1), código mnemotécnico
    ///                       asignado manualmente — mismo criterio que
    ///                       Inspeccion.Code)
    ///   Descripcion_Esp  -> Name
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos/maestros del sistema.
    /// </summary>
    public class Especialidad : IAuditableEntity
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
