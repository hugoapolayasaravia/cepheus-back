
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetPedidosPaginated
{
    public class GetPedidosPaginatedQuery : PagedRequest,
        IRequest<PagedResult<PedidoResponse>>
    {
        public string? Search { get; set; }
        public string? PlantaCode { get; set; }
        public string? Estado { get; set; }
        public string? SubCentroCostoCode { get; set; }
        public string? UsuarioCode { get; set; }
        public string? TipoPedidoCode { get; set; }
        public DateTime? FechaPedidoDesde { get; set; }
        public DateTime? FechaPedidoHasta { get; set; }
    }
}
