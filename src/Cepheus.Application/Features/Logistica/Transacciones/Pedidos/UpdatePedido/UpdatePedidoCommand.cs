// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/UpdatePedido/UpdatePedidoCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.UpdatePedido
{
    public record UpdatePedidoCommand(
        string PlantaCode,
        string Code,
        string TipoPedidoCode,
        string TipoValeCode,
        string TramiteCode,
        string SubCentroCostoCode,
        string TrabajadorCode,
        string? OrdenTrabajoCode,
        string UnidadNegocioCode,
        DateTime FechaEntrega,
        string? Observaciones,
        byte[] RowVersion
    ) : IRequest<PedidoResponse>;
}