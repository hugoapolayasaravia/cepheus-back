// Cepheus.Application/Features/Logistica/Transacciones/OrdenesCompra/Common/OrdenCompraResponse.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common
{
    public class OrdenCompraPedidoOrigenResponse
    {
        public string PedidoCode { get; set; } = default!;
        public int PedidoItemNumber { get; set; }
        public decimal CantidadTomada { get; set; }
    }

    public class OrdenCompraDetalleResponse
    {
        public string ArticuloCode { get; set; } = default!;
        public int ItemNumber { get; set; }
        public decimal CantidadArticulo { get; set; }
        public decimal PrecioArticulo { get; set; }
        public decimal DescuentoArticulo { get; set; }
        public decimal TotalArticulo { get; set; }
        public decimal CantidadEntregada { get; set; }
        public string SubCentroCostoCode { get; set; } = default!;
        public List<OrdenCompraPedidoOrigenResponse> Origenes { get; set; } = new();
    }

    public class OrdenCompraResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string TipoCompraCode { get; set; } = default!;
        public string ComprobantePagoCode { get; set; } = default!;
        public DateTime FechaEntrega { get; set; }
        public string ProveedorCode { get; set; } = default!;
        public string CompradorCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public string LugarEnvioCode { get; set; } = default!;
        public string FormaPagoCode { get; set; } = default!;
        public string TramiteCode { get; set; } = default!;
        public string? Observaciones1 { get; set; }
        public string? Observaciones2 { get; set; }
        public string? NotaCompraCode { get; set; }
        public string UnidadNegocioCode { get; set; } = default!;
        public bool EnviarCorreoProveedor { get; set; }
        public string Estado { get; set; } = default!;
        public string? AprobadoPor { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string? MotivoRetraso { get; set; }
        public decimal NetoCompra { get; set; }
        public decimal IgvCompra { get; set; }
        public decimal TotalCompra { get; set; }
        public decimal NoGravableCompra { get; set; }
        public decimal RentaCompra { get; set; }
        public decimal FonaviCompra { get; set; }
        public decimal ServicioCompra { get; set; }
        public decimal IgvExteriorCompra { get; set; }
        public List<OrdenCompraDetalleResponse> Detalles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}