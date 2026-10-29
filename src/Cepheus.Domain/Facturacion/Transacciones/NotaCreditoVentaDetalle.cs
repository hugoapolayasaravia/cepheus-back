using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Línea de la Nota de Crédito de Ventas.
    ///
    /// Legacy: dbo.DCreVentas. PK legacy (Numero_cve, item_cve) -> (PlantaCode, Number, Line).
    ///   item_cve        -> Line (correlativo char(3))
    ///   numero_gve      -> GuideNumber (guía de venta; opcional, código simple)
    ///   Codigo_tpr/Codigo_prd -> TipoProductoCode/ProductoCode (FK compuesta -> Producto)
    ///   Descripcion_cve -> Description
    ///   Cantidad_cve    -> Quantity
    ///   cantDesp_cve    -> DispatchedQuantity
    ///   PrecioSol_cve   -> UnitPrice (en la moneda del documento)
    ///   TotalSol_cve    -> Total
    ///   Clave           -> LineKey (línea del comprobante origen)
    ///   Codigo_ped      -> OrderCode (Orden de Trabajo de Venta)
    ///   Estado          -> ELIMINADO: se deriva del estado de la cabecera
    ///   FecProceso      -> ELIMINADO: variable de auditoría
    /// </summary>
    public class NotaCreditoVentaDetalle
    {
        public string PlantaCode { get; set; } = default!;
        public string Number { get; set; } = default!;
        public NotaCreditoVenta NotaCredito { get; set; } = default!;

        public string Line { get; set; } = default!;

        public string? GuideNumber { get; set; }

        public string TipoProductoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public string Description { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal DispatchedQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public int LineKey { get; set; }
        public string? OrderCode { get; set; }
    }
}
