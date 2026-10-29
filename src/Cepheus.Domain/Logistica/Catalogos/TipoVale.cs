using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Tipo de vale de salida de almacén (catálogo simple). Usado en el
    /// módulo de Logística.
    ///
    /// Legacy: dbo.TTipoVale (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Tva       -> Code (PK natural, char(3), autogenerado
    ///                        correlativamente por la aplicación)
    ///   Descripcion_Tva  -> Name
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos del sistema.
    /// </summary>
    public class TipoVale : IAuditableEntity
    {
        /// <summary>
        /// Código del tipo de vale (PK natural, 3 caracteres, correlativo).
        /// </summary>
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