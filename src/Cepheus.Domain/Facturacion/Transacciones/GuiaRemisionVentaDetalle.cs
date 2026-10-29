using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Línea de una guía de remisión. Legacy: DGuiVentas (PK Numero_gve + Item_gve).
    ///
    ///   Item_gve                 -> Line (1, 2, 3...)
    ///   Codigo_ped + clave       -> OrdenTrabajoCode + OrdenTrabajoItem (línea de la OT; null en guías sin OT)
    ///   Codigo_tpr, Codigo_prd   -> ProductoTipoCode, ProductoCode
    ///   Cantidad_gve             -> Quantity
    ///   tope_gve                 -> TopeQuantity (lo que quedaba por despachar de la programación al emitir)
    ///   precio_prd               -> UnitPrice (el de la línea de la OT, en la moneda de la OT)
    ///
    /// NO se migran: Codigo_pla/Codigo_cli/Codigo_obr (están en la cabecera), Color_gve, MtLineales, metros_gve,
    /// peso_prd, unidades_gve (sin regla en el PB entregado), preciosol_prd/brutosol_gve/brutodol_gve (importes que
    /// calcula el comprobante), Numero_Val, tipo_doc/numero_doc/codigo_tdo/Estado de línea (los fija el comprobante).
    /// </summary>
    public class GuiaRemisionVentaDetalle : IAuditableEntity
    {
        public string GuiaNumber { get; set; } = default!;
        public GuiaRemisionVenta Guia { get; set; } = default!;

        public int Line { get; set; }

        public string? OrdenTrabajoCode { get; set; }
        public int? OrdenTrabajoItem { get; set; }
        public OrdenTrabajoVentaDetalle? OrdenTrabajoDetalle { get; set; }

        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal? TopeQuantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
