using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Tipo de pedido (catálogo simple). Usado en el módulo de Logística.
    ///
    /// Legacy: dbo.TTipoPedido (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Tpe       -> Code (PK natural, char(2))
    ///   Descripcion_Tpe  -> Name
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos del sistema.
    /// </summary>
    public class TipoPedido : IAuditableEntity
    {
        /// <summary>
        /// Código del tipo de pedido (PK natural, 2 caracteres).
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