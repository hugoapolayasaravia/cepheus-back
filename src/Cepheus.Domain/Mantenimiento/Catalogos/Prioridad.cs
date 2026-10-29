using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Catalogos
{
    /// <summary>
    /// Prioridad de atención de una Orden de Trabajo (ej. "P1" = Inmediatamente).
    /// Catálogo del módulo Mantenimiento.
    ///
    /// Legacy: dbo.TPrioridades (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Pri       -> Code (PK natural, char(2), código mnemotécnico
    ///                       asignado manualmente — mismo criterio que
    ///                       Inspeccion.Code)
    ///   Descripcion_Pri  -> Name
    ///   Estado_Pri       -> reemplazado por IsActive estándar (no es el
    ///                       mismo "estado" que el de la Orden de Trabajo,
    ///                       es simplemente si la prioridad está habilitada)
    /// </summary>
    public class Prioridad : IAuditableEntity
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
