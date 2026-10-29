using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Maestros
{
    /// <summary>
    /// Verbo de acción usado para componer el código de una Actividad de
    /// mantenimiento (ej. "ACOPLAR", "LIMPIAR"). Maestro del módulo
    /// Mantenimiento — junto con ObjetoActividad, es la base de Actividad.
    ///
    /// Legacy: dbo.TVerbos (SQL Server). Se renombra de "Verbo" a
    /// "VerboActividad" para que el nombre sea autoexplicativo fuera de
    /// contexto (evita confundirlo con un verbo gramatical genérico).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Ver       -> Code (PK natural, char(3), código
    ///                       mnemotécnico manual — ej. "LIM" = Limpiar)
    ///   Descripcion_Ver  -> Name
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos/maestros del sistema.
    /// </summary>
    public class VerboActividad : IAuditableEntity
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
