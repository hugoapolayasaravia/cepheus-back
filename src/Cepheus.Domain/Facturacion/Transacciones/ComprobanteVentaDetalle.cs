using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Línea de un comprobante. Legacy: DFacVentas / DBolVentas (PK numero + item).
    ///
    ///   item_fve / item_bve        -> Line (1, 2, 3...)
    ///   numero_gve + clave         -> GuiaNumber + GuiaLine (línea de la guía facturada; null en comprobantes libres)
    ///   codigo_ped + clave         -> OrdenTrabajoCode + OrdenTrabajoItem
    ///   codigo_tpr, codigo_prd     -> ProductoTipoCode, ProductoCode
    ///   cantidad_*                 -> Quantity
    ///   precio*_*, total*_*        -> UnitPrice, Total (en la moneda del comprobante; el equivalente en soles se
    ///                                 calcula en la cabecera)
    ///
    /// NO se migran: cantdesp_* (despachado), numero_dct (documento de adelanto: pendiente), estado de línea.
    /// </summary>
    public class ComprobanteVentaDetalle : IAuditableEntity
    {
        public string TypeCode { get; set; } = default!;
        public string Number { get; set; } = default!;
        public ComprobanteVenta Comprobante { get; set; } = default!;

        public int Line { get; set; }

        public string? GuiaNumber { get; set; }
        public int? GuiaLine { get; set; }
        public GuiaRemisionVentaDetalle? GuiaDetalle { get; set; }

        public string? OrdenTrabajoCode { get; set; }
        public int? OrdenTrabajoItem { get; set; }

        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
