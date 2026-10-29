using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Línea de la OT de Ventas. Legacy: programacion.dbo.PedidoVentaDetalle.
    ///
    ///   Codigo_ped            -> OrdenTrabajoCode
    ///   Clave (IDENTITY)      -> Item (identity global: otras tablas del PB —programación, guías— la usan
    ///                            como referencia a la línea). Un mismo producto puede repetirse en una OT.
    ///   Codigo_tpr, Codigo_pru-> ProductoTipoCode, ProductoCode (FK compuesta -> Producto)
    ///   Codigo_neg            -> NegocioCode (FK -> Comunes.Negocio)
    ///   Piloto                -> NO se guarda: se deduce del negocio (CO=0 AG=1 BL=2 BA=3 PT=4 AD=5)
    ///   Cantidad_prd          -> Quantity
    ///   Cantidad_apr          -> ApprovedQuantity (la fija la aprobación de Créditos; 0 si se rechaza)
    ///   Cantidad_pro          -> ProgrammedQuantity (la actualiza la programación)
    ///   Cantidad_ent          -> DeliveredQuantity (la actualizan las guías: ad_actualizasaldos)
    ///   Precio_prd, Total_prd -> UnitPrice, Total (en productos tipo 23 —descuento— ambos negativos)
    ///   Estado                -> Status (P Pendiente | A Anulado)
    /// </summary>
    public class OrdenTrabajoVentaDetalle : IAuditableEntity
    {
        public string OrdenTrabajoCode { get; set; } = default!;
        public OrdenTrabajoVenta OrdenTrabajo { get; set; } = default!;

        public int Item { get; set; }

        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public string NegocioCode { get; set; } = default!;
        public Negocio Negocio { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal ApprovedQuantity { get; set; }
        public decimal ProgrammedQuantity { get; set; }
        public decimal DeliveredQuantity { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public EstadoDetalleOrdenTrabajoVenta Status { get; set; } = EstadoDetalleOrdenTrabajoVenta.Pendiente;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
