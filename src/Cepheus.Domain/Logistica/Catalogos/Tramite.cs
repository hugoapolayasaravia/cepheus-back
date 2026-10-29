using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Trámite asociado a una Orden de Compra (catálogo simple). Usado en el
    /// módulo de Logística.
    ///
    /// Legacy: dbo.TTramites (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Trm       -> Code (PK natural, char(1))
    ///   Descripcion_Trm  -> Name
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos del sistema.
    /// </summary>
    public class Tramite : IAuditableEntity
    {
        /// <summary>
        /// Código del trámite (PK natural, 1 carácter).
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