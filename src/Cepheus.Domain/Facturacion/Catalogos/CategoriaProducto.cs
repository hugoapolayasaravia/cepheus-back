using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Categoría de producto (ej. Concretos normales). Catálogo del módulo de
    /// Facturación y Ventas, referenciado por Producto.
    ///
    /// Legacy: dbo.TCategoriaProducto (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Cat      -> Code (PK natural, char(3), correlativo automático)
    ///   Descripcion_Cat -> Name
    ///   Codigo_firth    -> FirthCode (código en el sistema corporativo Firth; se
    ///                      conserva opcional, confirmar si sigue en uso)
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con el
    /// resto de catálogos.
    /// </summary>
    public class CategoriaProducto : IAuditableEntity
    {
        /// <summary>Código de la categoría (PK natural, 3 caracteres, correlativo).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        /// <summary>Código de la categoría en el sistema corporativo Firth (Codigo_firth). Opcional.</summary>
        public string? FirthCode { get; set; }

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
