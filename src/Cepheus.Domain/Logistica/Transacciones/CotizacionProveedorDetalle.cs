// Cepheus.Domain/Logistica/Transacciones/CotizacionProveedorDetalle.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Oferta de precio del proveedor por artículo. Legacy: dbo.DProvCotizacion.
    /// TotalLinea = Cantidad × Precio − Descuento (calculado, no legacy —
    /// se agrega para alimentar CotizacionProveedor.NetoCotizacion, mismo
    /// criterio que PedidoDetalle.TotalArticulo).
    /// </summary>
    public class CotizacionProveedorDetalle : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string CotizacionCode { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public CotizacionProveedor CotizacionProveedor { get; set; } = default!;

        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        public decimal CantidadArticulo { get; set; }
        public decimal PrecioArticulo { get; set; }
        public decimal DescuentoArticulo { get; set; }
        public decimal TotalLinea { get; set; }

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}