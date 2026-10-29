using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Catalogos
{
    /// <summary>
    /// Tipo de inspección de una Orden de Trabajo (ej. "Inspecciones de turno").
    /// Catálogo del módulo Mantenimiento.
    ///
    /// Legacy: dbo.TInspecciones (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Ins       -> Code (PK natural, char(2), código mnemotécnico
    ///                       asignado manualmente por el usuario — igual
    ///                       criterio que UnidadMedida.Code: el legacy no usa
    ///                       correlativo numérico puro, ej. "I1", así que no
    ///                       aplica SequentialCodeGenerator acá)
    ///   Descripcion_Ins  -> Name
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos/maestros del sistema.
    /// </summary>
    public class Inspeccion : IAuditableEntity
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
