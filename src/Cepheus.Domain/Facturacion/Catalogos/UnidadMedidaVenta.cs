using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Unidad de medida de venta (ej. M3, KG, UND). Catálogo del módulo de
    /// Facturación y Ventas, referenciado por Producto.
    ///
    /// Se nombra UnidadMedidaVenta (no UnidadMedida) porque Logística ya tiene
    /// Logistica.Catalogos.UnidadMedida para unidades de compra: mismo concepto de
    /// negocio, tabla y ciclo de vida propios del módulo de Ventas.
    ///
    /// Legacy: dbo.MProductos.Unidad_prd (varchar(5) NOT NULL, sin FK: era texto
    /// libre). Se modela como catálogo aparte para poder relacionarlo con FK, con
    /// la misma longitud de 5 caracteres del legacy.
    ///
    /// PENDIENTE: confirmar si existe una tabla legacy de unidades de venta
    /// (no se recibió) o si en verdad Unidad_prd era texto libre; en ese caso, la
    /// carga inicial del catálogo debe basarse en los valores distintos usados en
    /// MProductos (ver SQL de validación).
    /// </summary>
    public class UnidadMedidaVenta : IAuditableEntity
    {
        /// <summary>Código de la unidad de medida (PK natural, hasta 5 caracteres, ej. "M3", "KG", "UND").</summary>
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
