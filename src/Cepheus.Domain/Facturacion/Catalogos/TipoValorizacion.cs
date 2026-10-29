using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Tipo de valorización (catálogo). Define cada cuántos días se valoriza
    /// (liquida) la facturación de una obra (Obra.TipoValorizacionCode).
    ///
    /// Legacy: dbo.TTipValorizacion (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_val       -> Code (PK natural, char(1), correlativo)
    ///   Descripcion_val  -> Name (char(10) en el legacy -> varchar(10) acá; los
    ///                       espacios de relleno del char se recortan al migrar)
    ///   Dias_val         -> Days (int NOT NULL, periodicidad en días)
    ///
    /// IsActive no existe en el legacy; se agrega por consistencia con el resto
    /// de catálogos del sistema.
    /// </summary>
    public class TipoValorizacion : IAuditableEntity
    {
        /// <summary>
        /// Código del tipo de valorización (PK natural, 1 carácter).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        /// <summary>
        /// Periodicidad de la valorización, en días.
        /// </summary>
        public int Days { get; set; }

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
