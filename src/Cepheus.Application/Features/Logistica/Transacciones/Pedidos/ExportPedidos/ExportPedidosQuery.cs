using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.ExportPedidos
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado de pedidos, sin paginar.</summary>
    public class ExportPedidosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? PlantaCode { get; set; }
        public string? Estado { get; set; }
        public string? TrabajadorCode { get; set; }
        public string? SubCentroCostoCode { get; set; }
        public string? TipoPedidoCode { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? FechaProcesoDesde { get; set; }
        public DateTime? FechaProcesoHasta { get; set; }
        public DateTime? FechaEntregaDesde { get; set; }
        public DateTime? FechaEntregaHasta { get; set; }
    }
}
