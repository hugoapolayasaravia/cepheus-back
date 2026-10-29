using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Línea de la Solicitud de Nota de Crédito.
    ///
    /// Legacy: dbo.DCreVentasSol. PK legacy (Codigo_snc, Ano_snc, Mes_snc, Codigo_tpr,
    /// Codigo_prd, Clave).
    ///   Codigo_tpr, Codigo_prd -> TipoProductoCode, ProductoCode (FK compuesta -> Producto)
    ///   Clave                  -> LineKey (identifica la línea del comprobante origen)
    ///   item_cve               -> Line (correlativo de línea, char(3))
    ///   Cantidad_snc           -> Quantity
    ///   Precio_snc             -> UnitPrice
    ///   Total_snc              -> Total (Quantity x UnitPrice, redondeado a 2 decimales)
    ///   Codigo_ped             -> OrderCode (Orden de Trabajo de Ventas; el modelo de
    ///                             Pedidos aún no está migrado, queda como código simple)
    /// </summary>
    public class SolicitudNotaCreditoDetalle
    {
        public string Code { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public SolicitudNotaCredito Solicitud { get; set; } = default!;

        public string TipoProductoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public int LineKey { get; set; }
        public string Line { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public string? OrderCode { get; set; }
    }
}
