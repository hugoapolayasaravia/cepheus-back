using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Relación Artículo-Proveedor: qué proveedores ofrecen qué artículo en
    /// qué planta, con precio de convenio opcional. Tabla puente que se usará
    /// cuando se implemente el módulo de Compras (Órdenes de Compra
    /// necesitarán saber qué proveedores pueden surtir un artículo).
    ///
    /// Legacy: dbo.MArticulo_Proveedor (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Pla            -> PlantaCode (FK -> Comunes.Planta.Code).
    ///                            NOTA: en el legacy "Codigo_Pla" se usa tanto
    ///                            para "Planta" (acá, char2) como para "Plan"
    ///                            (en MArticulos, char3) — mismo nombre de
    ///                            columna, dos catálogos distintos. Acá se
    ///                            confirmó que es Planta.
    ///   Codigo_Art            -> ArticuloCode (FK -> Articulo.Code)
    ///   Codigo_Prv            -> ProveedorCode (FK -> Proveedor.Code)
    ///   Ind_convenio          -> IsAgreement (bool, 'S'/'N' -> true/false)
    ///   imp_precio_convenio   -> AgreementPrice (nullable — precio especial
    ///                            pactado con el proveedor, si aplica)
    ///
    /// PK compuesta (Planta, Artículo, Proveedor) — no aplica el patrón de
    /// correlativo string de los catálogos/maestros con código propio.
    ///
    /// Sin campos de auditoría ni Estado/IsActive en el legacy (tabla puente
    /// pura). Se agrega auditoría estándar por consistencia con el resto del
    /// sistema; no se agrega IsActive porque no hay noción de "baja" para
    /// esta relación — se elimina directamente cuando el proveedor deja de
    /// surtir ese artículo en esa planta.
    /// </summary>
    public class ArticuloProveedor
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public bool IsAgreement { get; set; }
        public decimal? AgreementPrice { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
