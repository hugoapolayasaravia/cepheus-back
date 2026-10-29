// Cepheus.Application/Features/Logistica/Transacciones/OrdenCompraDetalles/DeleteOrdenCompraDetalle/DeleteOrdenCompraDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.OrdenesCompra.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.OrdenCompraDetalles.DeleteOrdenCompraDetalle
{
    public class DeleteOrdenCompraDetalleCommandHandler : IRequestHandler<DeleteOrdenCompraDetalleCommand, OrdenCompraResponse>
    {
        private readonly IUnitOfWork _uow;

        public DeleteOrdenCompraDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<OrdenCompraResponse> Handle(DeleteOrdenCompraDetalleCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var ordenCode = request.OrdenCompraCode.Trim().ToUpperInvariant();
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            var orden = await _uow.Logistica.Transacciones.OrdenesCompra.Query()
                .Include(o => o.Detalles).ThenInclude(d => d.Origenes)
                .FirstOrDefaultAsync(o => o.PlantaCode == plantaCode && o.Code == ordenCode, cancellationToken);

            if (orden is null)
            {
                throw new KeyNotFoundException($"Orden de Compra {plantaCode}/{ordenCode} no encontrada.");
            }

            if (orden.Estado != EstadoOrdenCompra.Pendiente)
            {
                throw new InvalidOperationException($"La Orden de Compra está en estado '{orden.Estado}' y ya no admite eliminar líneas.");
            }

            var detalle = orden.Detalles.FirstOrDefault(d => d.ArticuloCode == articuloCode);
            if (detalle is null)
            {
                throw new KeyNotFoundException($"Artículo {articuloCode} no encontrado en la Orden de Compra {plantaCode}/{ordenCode}.");
            }

            if (orden.Detalles.Count == 1)
            {
                throw new InvalidOperationException("La Orden de Compra debe conservar al menos una línea.");
            }

            var pedidoLineasAfectadas = detalle.Origenes.Select(o => (o.PlantaCode, o.PedidoCode, o.PedidoItemNumber)).ToList();

            orden.Detalles.Remove(detalle);
            _uow.Logistica.Transacciones.OrdenCompraDetalles.Remove(detalle);

            await OrdenCompraTotalsCalculator.RecalculateAsync(_uow, orden, cancellationToken);

            if (pedidoLineasAfectadas.Count > 0)
            {
                await OrdenCompraTotalsCalculator.RecalculatePedidoCantidadEnCompraAsync(_uow, pedidoLineasAfectadas, cancellationToken);
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return OrdenCompraMapper.Map(orden);
        }
    }
}