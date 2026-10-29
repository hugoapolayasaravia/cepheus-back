using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Tipo de operación de venta (catálogo tributario). Catálogo del módulo de
    /// Facturación y Ventas, referenciado por Producto.
    ///
    /// Legacy: dbo.COM_OPERACION_TIPO (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   COD_TIP_OPERACION -> Code (PK natural, char(2); se ingresa manualmente)
    ///   DES_TIP_OPERACION -> Name (varchar(255))
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia.
    /// </summary>
    public class TipoOperacion : IAuditableEntity
    {
        /// <summary>Código del tipo de operación (PK natural, 2 caracteres).</summary>
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
