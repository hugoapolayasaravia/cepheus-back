// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/Common/PedidoDetalleResponse.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common
{
    public class PedidoDetalleResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string PedidoCode { get; set; } = default!;
        public int ItemNumber { get; set; }

        public string? ArticuloCode { get; set; }
        public string DescripcionArticulo { get; set; } = default!;

        public string UnidadMedidaCode { get; set; } = default!;
        public decimal PrecioArticulo { get; set; }
        public decimal CantidadArticulo { get; set; }
        public decimal TotalArticulo { get; set; }

        public string EstadoPedidoDetalle { get; set; } = default!;

        public string? OrdenCompraCode { get; set; }
        public string? ProveedorCode { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}