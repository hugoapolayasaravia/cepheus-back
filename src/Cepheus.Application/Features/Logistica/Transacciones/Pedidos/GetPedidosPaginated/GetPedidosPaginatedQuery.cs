// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/GetPedidosPaginated/GetPedidosPaginatedQuery.cs
using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.GetPedidosPaginated
{
    public class GetPedidosPaginatedQuery : PagedRequest, IRequest<PagedResult<PedidoResponse>>
    {
        public string? Search { get; set; }
        public string? PlantaCode { get; set; }
        public string? Estado { get; set; }
        public string? TrabajadorCode { get; set; }
        public string? SubCentroCostoCode { get; set; }
        public DateTime? FechaEntregaDesde { get; set; }
        public DateTime? FechaEntregaHasta { get; set; }
    }
}