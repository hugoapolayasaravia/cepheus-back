// Cepheus.Application/Features/Logistica/Transacciones/PedidoDetalles/UpdatePedidoDetalle/UpdatePedidoDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.UpdatePedidoDetalle
{
    public record UpdatePedidoDetalleCommand(
        string PlantaCode,
        string PedidoCode,
        int ItemNumber,
        string? ArticuloCode,
        string DescripcionArticulo,
        string UnidadMedidaCode,
        decimal PrecioArticulo,
        decimal CantidadArticulo,
        string? ProveedorCode,
        byte[] RowVersion
    ) : IRequest<PedidoResponse>;
}