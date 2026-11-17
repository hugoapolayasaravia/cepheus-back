// Cepheus.Application/Features/Logistica/Transacciones/Pedidos/ChangeEstadoPedido/ChangeEstadoPedidoCommand.cs
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.ChangeEstadoPedido
{
    public record ChangeEstadoPedidoCommand(
        string PlantaCode,
        string Code,
        string NuevoEstado,
        // «Aprobación por usuario» (legacy): quien aprueba o devuelve a Pendiente se identifica con login y clave.
        string? AprobadorLogin = null,
        string? AprobadorClave = null
    ) : IRequest<PedidoResponse>;
}