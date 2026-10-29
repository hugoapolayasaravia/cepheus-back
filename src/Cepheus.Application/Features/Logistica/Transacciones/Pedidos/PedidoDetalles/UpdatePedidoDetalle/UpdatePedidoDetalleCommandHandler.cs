// Cepheus.Application/Features/Logistica/Transacciones/PedidoDetalles/UpdatePedidoDetalle/UpdatePedidoDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.UpdatePedidoDetalle
{
    public class UpdatePedidoDetalleCommandHandler : IRequestHandler<UpdatePedidoDetalleCommand, PedidoResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdatePedidoDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PedidoResponse> Handle(UpdatePedidoDetalleCommand request, CancellationToken cancellationToken)
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
                    $"El Pedido está en estado '{pedido.EstadoPedido}' y ya no admite editar líneas.");
            }

            var linea = pedido.Detalles.FirstOrDefault(d => d.ItemNumber == request.ItemNumber);
            if (linea is null)
            {
                throw new KeyNotFoundException(
                    $"Línea {request.ItemNumber} del Pedido {plantaCode}/{pedidoCode} no encontrada.");
            }

            linea.ArticuloCode = string.IsNullOrWhiteSpace(request.ArticuloCode) ? null : request.ArticuloCode.Trim().ToUpperInvariant();
            linea.DescripcionArticulo = request.DescripcionArticulo.Trim();
            linea.UnidadMedidaCode = request.UnidadMedidaCode.Trim().ToUpperInvariant();
            linea.PrecioArticulo = request.PrecioArticulo;
            linea.CantidadArticulo = request.CantidadArticulo;
            linea.TotalArticulo = PedidoTotalsCalculator.CalculateLineTotal(request.PrecioArticulo, request.CantidadArticulo);
            linea.ProveedorCode = string.IsNullOrWhiteSpace(request.ProveedorCode) ? null : request.ProveedorCode.Trim().ToUpperInvariant();
            linea.RowVersion = request.RowVersion;

            _uow.Logistica.Transacciones.PedidoDetalles.Update(linea);

            await PedidoTotalsCalculator.RecalculateAsync(_uow, pedido, cancellationToken);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La línea fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CreatePedidoCommandHandler.Map(pedido);
        }
    }
}