// Cepheus.Application/Features/Logistica/Transacciones/PedidoDetalles/CreatePedidoDetalle/CreatePedidoDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Pedidos.CreatePedido;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.CreatePedidoDetalle
{
    public class CreatePedidoDetalleCommandHandler : IRequestHandler<CreatePedidoDetalleCommand, PedidoResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreatePedidoDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PedidoResponse> Handle(CreatePedidoDetalleCommand request, CancellationToken cancellationToken)
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
                    $"El Pedido está en estado '{pedido.EstadoPedido}' y ya no admite agregar líneas.");
            }

            var nextItem = pedido.Detalles.Count == 0 ? 1 : pedido.Detalles.Max(d => d.ItemNumber) + 1;

            var linea = new PedidoDetalle
            {
                PlantaCode = plantaCode,
                PedidoCode = pedidoCode,
                ItemNumber = nextItem,
                ArticuloCode = string.IsNullOrWhiteSpace(request.ArticuloCode) ? null : request.ArticuloCode.Trim().ToUpperInvariant(),
                DescripcionArticulo = request.DescripcionArticulo.Trim(),
                UnidadMedidaCode = request.UnidadMedidaCode.Trim().ToUpperInvariant(),
                PrecioArticulo = request.PrecioArticulo,
                CantidadArticulo = request.CantidadArticulo,
                TotalArticulo = PedidoTotalsCalculator.CalculateLineTotal(request.PrecioArticulo, request.CantidadArticulo),
                EstadoPedidoDetalle = EstadoPedidoDetalle.Pendiente,
                ProveedorCode = string.IsNullOrWhiteSpace(request.ProveedorCode) ? null : request.ProveedorCode.Trim().ToUpperInvariant()
            };

            pedido.Detalles.Add(linea);
            await _uow.Logistica.Transacciones.PedidoDetalles.AddAsync(linea, cancellationToken);

            await PedidoTotalsCalculator.RecalculateAsync(_uow, pedido, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);

            return CreatePedidoCommandHandler.Map(pedido);
        }
    }
}