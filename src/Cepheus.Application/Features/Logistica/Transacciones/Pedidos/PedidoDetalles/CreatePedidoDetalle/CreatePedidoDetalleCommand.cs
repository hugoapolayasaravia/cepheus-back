// Cepheus.Application/Features/Logistica/Transacciones/PedidoDetalles/CreatePedidoDetalle/CreatePedidoDetalleCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.CreatePedidoDetalle
{
    /// <summary>Agrega una línea a un Pedido existente en estado Pendiente.</summary>
    public record CreatePedidoDetalleCommand(
        string PlantaCode,
        string PedidoCode,
        string? ArticuloCode,
        string DescripcionArticulo,
        string UnidadMedidaCode,
        decimal PrecioArticulo,
        decimal CantidadArticulo,
        string? ProveedorCode
    ) : IRequest<PedidoResponse>;
}