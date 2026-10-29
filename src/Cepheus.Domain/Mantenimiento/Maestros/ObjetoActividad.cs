using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Maestros
{
    /// <summary>
    /// Objeto sobre el que se aplica el verbo de una Actividad de
    /// mantenimiento (ej. "BOBINAS", "ENFRIADOR ACEITE MOTOR"). Maestro del
    /// módulo Mantenimiento — junto con VerboActividad, es la base de
    /// Actividad.
    ///
    /// Legacy: dbo.TSustantivos (SQL Server). Se renombra de "Sustantivo" a
    /// "ObjetoActividad" (simétrico con VerboActividad) para expresar el rol
    /// de negocio en vez de la categoría gramatical.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Sus       -> Code (PK natural, char(3), código
    ///                       mnemotécnico manual)
    ///   Descripcion_Sus  -> Name (en el legacy es varchar(50) NULL; se
    ///                       vuelve obligatorio acá por consistencia con el
    ///                       resto de catálogos/maestros del sistema)
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos/maestros del sistema.
    /// </summary>
    public class ObjetoActividad : IAuditableEntity
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
