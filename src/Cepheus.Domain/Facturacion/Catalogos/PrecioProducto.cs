using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Precio de producto: matriz de precios de venta por producto, flete y
    /// moneda, usada para poblar Precio_prd al agregar un producto a una
    /// cotización. Catálogo del módulo de Facturación y Ventas.
    ///
    /// Legacy: dbo.DPrecioProductos (SQL Server). PK compuesta de 5 columnas
    /// (codigo_fle, codigo_tpr, codigo_prd, codigo_mon, moneda). En el modelo
    /// nuevo la moneda se relaciona con Comunes.Moneda (código ISO 4217
    /// alfabético: PEN, USD) y la PK queda de 4 columnas: flete + producto
    /// (tipo + código) + moneda.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   codigo_fle          -> FleteCode (FK -> Flete, parte de la PK)
    ///   codigo_tpr+codigo_prd -> ProductoTipoCode + ProductoCode (FK compuesta
    ///                         -> Producto, parte de la PK)
    ///   moneda / codigo_mon -> CurrencyCode (FK -> Comunes.Moneda.Code, parte
    ///                         de la PK). El legacy tenía dos columnas char(1)
    ///                         (codigo_mon y moneda); se unifican en una sola
    ///                         moneda. La carga inicial (ETL) necesita la tabla
    ///                         de equivalencia char(1) -> código ISO, igual que
    ///                         Cliente.CurrencyCode y Obra.CreditCurrencyCode.
    ///   monto               -> Amount (numeric(18,4))
    ///   monto_tra           -> TransportAmount (numeric(18,4))
    ///   monto_fle           -> FreightAmount (numeric(18,4))
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

        /// <summary>Moneda del precio (FK -> Comunes.Moneda.Code, ISO 4217).</summary>
        public string CurrencyCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

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
