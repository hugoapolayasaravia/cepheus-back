using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Línea de la Nota de Débito de Ventas.
    ///
    /// Legacy: dbo.DDebVentas. PK legacy (Numero_dve, item_dve) -> (PlantaCode, Number, Line).
    ///   item_dve        -> Line (correlativo char(3))
    ///   Codigo_tpr/Codigo_prd -> TipoProductoCode/ProductoCode (FK compuesta -> Producto)
    ///   Descripcion_dve -> Description
    ///   Cantidad_dve    -> Quantity
    ///   PrecioSol_dve   -> UnitPrice (en la moneda del documento)
    ///   TotalSol_dve    -> Total
    ///   Clave           -> LineKey
    ///   CantDesp_dve, Estado, FecProceso -> ELIMINADOS (no se usan / se derivan de la cabecera / auditoría)
    /// </summary>
    public class NotaDebitoVentaDetalle
    {
        public string PlantaCode { get; set; } = default!;
        public string Number { get; set; } = default!;
        public NotaDebitoVenta NotaDebito { get; set; } = default!;

        public string Line { get; set; } = default!;

        public string TipoProductoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public string Description { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public int LineKey { get; set; }
    }
}
