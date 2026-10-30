using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.GetPedidoDetalles;

public sealed record GetPedidoDetallesQuery(
    string PlantaCode,
    string PedidoCode
) : IRequest<IReadOnlyList<PedidoDetalleResponse>>;