// Cepheus.Application/Features/Logistica/Transacciones/PedidoDetalles/DeletePedidoDetalle/DeletePedidoDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.DeletePedidoDetalle
{
    public record DeletePedidoDetalleCommand(
        string PlantaCode,
        string PedidoCode,
        int ItemNumber
    ) : IRequest<PedidoResponse>;
}