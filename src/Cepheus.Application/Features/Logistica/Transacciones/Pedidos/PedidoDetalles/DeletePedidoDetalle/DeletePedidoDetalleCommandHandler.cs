// Cepheus.Application/Features/Logistica/Transacciones/PedidoDetalles/DeletePedidoDetalle/DeletePedidoDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.DeletePedidoDetalle
{
    /// <summary>
    /// Hard delete, mismo criterio que ArticuloProveedor/OTRMaterial —
    /// borrado real de línea, no soft-delete. Exige al menos una línea
    /// restante (no se permite un Pedido sin detalle).
    /// </summary>
    public class DeletePedidoDetalleCommandHandler : IRequestHandler<DeletePedidoDetalleCommand, PedidoResponse>
    {
        private readonly IUnitOfWork _uow;

        public DeletePedidoDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PedidoResponse> Handle(DeletePedidoDetalleCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var pedidoCode = request.PedidoCode.Trim().ToUpperInvariant();

            var pedido = await _uow.Logistica.Transacciones.Pedidos.Query()
                .Include(p => p.Detalles)
                .FirstOrDefaultAsync(p => p.PlantaCode == plantaCode && p.Code == pedidoCode, cancellationToken);

            if (pedido is null)
            {
                throw new KeyNotFoundException($"Pedido {plantaCode}/{pedidoCode} no encontrado.");
            }

            if (pedido.EstadoPedido != EstadoPedido.Pendiente)
            {
                throw new InvalidOperationException(
                    $"El Pedido está en estado '{pedido.EstadoPedido}' y ya no admite eliminar líneas.");
            }

            var linea = pedido.Detalles.FirstOrDefault(d => d.ItemNumber == request.ItemNumber);
            if (linea is null)
            {
                throw new KeyNotFoundException(
                    $"Línea {request.ItemNumber} del Pedido {plantaCode}/{pedidoCode} no encontrada.");
            }

            if (pedido.Detalles.Count == 1)
            {
                throw new InvalidOperationException("El Pedido debe conservar al menos una línea de detalle.");
            }

            pedido.Detalles.Remove(linea);
            _uow.Logistica.Transacciones.PedidoDetalles.Remove(linea);

            await PedidoTotalsCalculator.RecalculateAsync(_uow, pedido, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return CreatePedidoCommandHandler.Map(pedido);
        }
    }
}