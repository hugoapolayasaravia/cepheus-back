using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Precio de producto: matriz de precios de venta por producto, flete y
    /// moneda, usada para poblar Precio_prd al agregar un producto a una
    /// cotización. Catálogo del módulo de Facturación y Ventas.
    ///
    /// Legacy: dbo.DPrecioProductos (SQL Server). PK compuesta de 5 columnas
    /// (codigo_fle, codigo_tpr, codigo_prd, codigo_mon, moneda): se mantiene
    /// como PK compuesta, no tiene código natural propio.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_fle          -> FleteCode (FK -> Flete, parte de la PK)
    ///   codigo_tpr+codigo_prd -> ProductoTipoCode + ProductoCode (FK compuesta
    ///                         -> Producto, parte de la PK)
    ///   codigo_mon          -> CurrencyTypeCode (char(1), parte de la PK)
    ///   moneda              -> CurrencyCode (char(1), default 'S', parte de
    ///                         la PK)
    ///   monto               -> Amount (numeric(18,4))
    ///   monto_tra           -> TransportAmount (numeric(18,4))
    ///   monto_fle           -> FreightAmount (numeric(18,4))
    ///
    /// PENDIENTE CONFIRMAR: el legacy declara codigo_mon Y moneda como dos
    /// columnas char(1) distintas, ambas parte de la PK, sin comentario que
    /// explique la diferencia entre ellas (posible duplicación histórica,
    /// mismo patrón ya visto en Comunes.ControlVentas con Anomes_cierre/
    /// AnnoMes_cie). Se preservan ambas tal cual hasta poder confirmar si son
    /// redundantes o tienen significados distintos (ej. tipo de cambio vs.
    /// moneda de la lista de precios). Tampoco se relacionan con
    /// Comunes.Moneda (código ISO alfabético PEN/USD) porque acá son
    /// char(1): requiere tabla de equivalencia para el ETL, igual que
    /// Cliente.CurrencyCode/Obra.CreditCurrencyCode.
    ///
    /// IsActive no existe en el legacy; no se agrega porque esta tabla es una
    /// matriz de precios sin ciclo de vida propio (se reemplaza el precio, no
    /// se desactiva una fila).
    /// </summary>
    public class PrecioProducto : IAuditableEntity
    {
        public string FleteCode { get; set; } = default!;
        public Flete Flete { get; set; } = default!;

        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Maestros.Producto Producto { get; set; } = default!;

        /// <summary>Ver nota PENDIENTE CONFIRMAR en el comentario de la clase.</summary>
        public string CurrencyTypeCode { get; set; } = default!;

        public string CurrencyCode { get; set; } = "S";

        public decimal Amount { get; set; }
        public decimal TransportAmount { get; set; }
        public decimal FreightAmount { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
