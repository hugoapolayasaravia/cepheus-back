// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/Common/CotizacionResponse.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common
{
    public class CotizacionPedidoOrigenResponse
    {
        public string PedidoCode { get; set; } = default!;
        public int PedidoItemNumber { get; set; }
        public decimal CantidadTomada { get; set; }
    }

    public class CotizacionDetalleResponse
    {
        public string ArticuloCode { get; set; } = default!;
        public int ItemNumber { get; set; }
        public decimal CantidadArticulo { get; set; }
        public List<CotizacionPedidoOrigenResponse> Origenes { get; set; } = new();
    }

    public class CotizacionProveedorDetalleResponse
    {
        public string ArticuloCode { get; set; } = default!;
        public decimal CantidadArticulo { get; set; }
        public decimal PrecioArticulo { get; set; }
        public decimal DescuentoArticulo { get; set; }
        public decimal TotalLinea { get; set; }
    }

    public class CotizacionProveedorResponse
    {
        public string ProveedorCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public decimal NetoCotizacion { get; set; }
        public decimal IgvCotizacion { get; set; }
        public decimal TotalCotizacion { get; set; }
        public string Observaciones { get; set; } = default!;
        public string Estado { get; set; } = default!;
        public DateTime? FechaRespuesta { get; set; }
        public List<CotizacionProveedorDetalleResponse> Detalles { get; set; } = new();
        public byte[] RowVersion { get; set; } = default!;
    }

    public class CotizacionResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public DateTime FechaLimite { get; set; }
        public string Estado { get; set; } = default!;
        public string Observaciones { get; set; } = default!;
        public string? OriginalCode { get; set; }
        public DateTime? FechaCierre { get; set; }
        public List<CotizacionDetalleResponse> Detalles { get; set; } = new();
        public List<CotizacionProveedorResponse> Proveedores { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}