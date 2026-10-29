using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Catalogos
{
    /// <summary>
    /// Falla que origina la parada no programada de una unidad/equipo
    /// (ver UnidadParada). Catálogo del módulo Mantenimiento.
    ///
    /// Legacy: dbo.TFallas (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Fal  -> Code (PK natural, char(5), código mnemotécnico
    ///                  asignado manualmente)
    ///   Nombre_Fal  -> Name (varchar(60))
    ///
    /// El legacy no tiene columna de estado; se agrega IsActive estándar
    /// para poder deshabilitar una falla sin borrarla (está referenciada
    /// por UnidadParada.FallaCode).
    /// </summary>
    public class Falla : IAuditableEntity
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
