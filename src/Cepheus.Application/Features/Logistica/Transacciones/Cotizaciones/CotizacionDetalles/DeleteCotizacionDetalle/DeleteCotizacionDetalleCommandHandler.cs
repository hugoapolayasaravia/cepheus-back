// Cepheus.Application/Features/Logistica/Transacciones/CotizacionDetalles/DeleteCotizacionDetalle/DeleteCotizacionDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionDetalles.DeleteCotizacionDetalle
{
    /// <summary>Hard delete. Al borrar la línea, sus CotizacionPedidoOrigen se van en cascada (ver Configuration) y se recalcula CantidadCotizada de los Pedidos afectados.</summary>
    public class DeleteCotizacionDetalleCommandHandler : IRequestHandler<DeleteCotizacionDetalleCommand, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCotizacionDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(DeleteCotizacionDetalleCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var cotizacionCode = request.CotizacionCode.Trim().ToUpperInvariant();
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Logistica.Transacciones.Cotizaciones.Query()
                .Include(c => c.Detalles).ThenInclude(d => d.Origenes)
                .FirstOrDefaultAsync(c => c.PlantaCode == plantaCode && c.Code == cotizacionCode, cancellationToken);

            if (cotizacion is null)
            {
                throw new KeyNotFoundException($"Cotización {plantaCode}/{cotizacionCode} no encontrada.");
            }

            if (cotizacion.Estado != EstadoCotizacion.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Cotización está en estado '{cotizacion.Estado}' y ya no admite eliminar artículos.");
            }

            var detalle = cotizacion.Detalles.FirstOrDefault(d => d.ArticuloCode == articuloCode);
            if (detalle is null)
            {
                throw new KeyNotFoundException($"Artículo {articuloCode} no encontrado en la Cotización {plantaCode}/{cotizacionCode}.");
            }

            if (cotizacion.Detalles.Count == 1)
            {
                throw new InvalidOperationException("La Cotización debe conservar al menos un artículo.");
            }

            var pedidoLineasAfectadas = detalle.Origenes
                .Select(o => (o.PlantaCode, o.PedidoCode, o.PedidoItemNumber))
                .ToList();

            cotizacion.Detalles.Remove(detalle);
            _uow.Logistica.Transacciones.CotizacionDetalles.Remove(detalle);

            if (pedidoLineasAfectadas.Count > 0)
            {
                await CotizacionCalculators.RecalculatePedidoCantidadCotizadaAsync(_uow, pedidoLineasAfectadas, cancellationToken);
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return CotizacionMapper.Map(cotizacion);
        }
    }
}