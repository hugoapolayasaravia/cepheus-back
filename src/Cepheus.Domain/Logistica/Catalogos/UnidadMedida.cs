using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Unidad de medida de artículos (ej. Unidad, Kilogramo, Caja). Catálogo
    /// simple usado en el módulo de Logística.
    ///
    /// Legacy: dbo.TUnidades (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_uni       -> Code (PK natural, char(2))
    ///   Descripcion_Uni  -> Name
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos del sistema.
    /// </summary>
    public class UnidadMedida : IAuditableEntity
    {
        /// <summary>
        /// Código de la unidad de medida (PK natural, 2 caracteres).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string IndDecimal { get; set; } = default!;

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