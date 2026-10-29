// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/ChangeEstadoPedido/ChangeEstadoPedidoCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.ChangeEstadoPedido
{
    public record ChangeEstadoPedidoCommand(
        string PlantaCode,
        string Code,
        string NuevoEstado
    ) : IRequest<PedidoResponse>;
}