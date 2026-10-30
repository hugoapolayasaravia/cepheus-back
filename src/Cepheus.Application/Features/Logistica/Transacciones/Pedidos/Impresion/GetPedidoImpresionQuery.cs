
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Impresion;

public sealed record GetPedidoImpresionQuery(
    string PlantaCode,
    string PedidoCode
) : IRequest<PedidoImpresionResponse?>;
