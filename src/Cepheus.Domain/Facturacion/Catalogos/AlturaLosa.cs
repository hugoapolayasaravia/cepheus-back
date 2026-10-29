using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Altura de losa: parámetro técnico usado en el metrado de cotizaciones de
    /// losas aligeradas (concreto + bovedilla, con variante opcional en
    /// poliestireno). Catálogo del módulo de Facturación y Ventas.
    ///
    /// Legacy: dbo.TAlturaLosa (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_alt   -> Code (PK natural, char(2), correlativo)
    ///   Nombre_alt   -> Name (varchar(50))
    ///   Valor_Alt    -> Value (decimal(18,2), altura de la losa)
    ///   Ancho_Alt    -> Width (decimal(18,2), ancho asociado)
    ///   Codigo_tpr + Codigo_prd -> ConcreteProductoTipoCode + ConcreteProductoCode
    ///                  (FK compuesta -> Producto, obligatoria: producto de
    ///                  concreto usado en esta altura de losa)
    ///   Codigo_tprP + Codigo_prdP -> PolystyreneProductoTipoCode +
    ///                  PolystyreneProductoCode (FK compuesta -> Producto,
    ///                  opcional: variante en poliestireno)
    ///   Valor_AltP   -> PolystyreneValue (decimal(18,2))
    ///   Ancho_AltP   -> PolystyreneWidth (decimal(18,2))
    ///
    /// PENDIENTE CONFIRMAR: en el legacy Valor_AltP/Ancho_AltP son NOT NULL
    /// aunque Codigo_tprP/Codigo_prdP son NULL (una losa puede no tener
    /// variante de poliestireno pero igual exige un valor/ancho en 0). Se
    /// preserva la misma inconsistencia (con default 0) en vez de forzarlas a
    /// nullable, hasta confirmar si en la práctica ambas siempre vienen juntas.
    ///
    /// IsActive no existe en el legacy; se agrega por consistencia con el
    /// resto de catálogos del sistema.
    /// </summary>
    public class AlturaLosa : IAuditableEntity
    {
        /// <summary>Código de la altura de losa (PK natural, 2 caracteres, correlativo).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public decimal Value { get; set; }
        public decimal Width { get; set; }

        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Maestros.Producto Productos { get; set; } = default!;

        public string? PolystyreneProductoTipoCode { get; set; }
        public string? PolystyreneProductoCode { get; set; }
        public Maestros.Producto? PolystyreneProducto { get; set; }

        public decimal PolystyreneValue { get; set; }
        public decimal PolystyreneWidth { get; set; }

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
