// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/Common/PedidoResponse.cs
namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common
{
    public class PedidoResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string CodPlanta { get; set; } = default!;

        public string TipoPedidoCode { get; set; } = default!;
        public string? TipoValeCode { get; set; }
        public string TramiteCode { get; set; } = default!;
        public string SubCentroCostoCode { get; set; } = default!;
        public string TrabajadorCode { get; set; } = default!;
        public string? OrdenTrabajoCode { get; set; }
        public string UnidadNegocioCode { get; set; } = default!;

        public DateTime FechaEntrega { get; set; }

        public decimal NetoPedido { get; set; }
        public decimal IgvPedido { get; set; }
        public decimal TotalPedido { get; set; }

        public string EstadoPedido { get; set; } = default!;
        public string Observaciones { get; set; } = default!;

        public string? AprobadoPor { get; set; }
        public DateTime? FechaAprobacion { get; set; }
        public string? CompradoPor { get; set; }
        public DateTime? FechaCompra { get; set; }

        public List<PedidoDetalleResponse> Detalles { get; set; } = new();

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}