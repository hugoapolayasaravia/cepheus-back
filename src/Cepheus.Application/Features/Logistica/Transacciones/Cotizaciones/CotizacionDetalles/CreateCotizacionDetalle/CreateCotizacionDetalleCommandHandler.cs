// Cepheus.Application/Features/Logistica/Transacciones/CotizacionDetalles/CreateCotizacionDetalle/CreateCotizacionDetalleCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CotizacionDetalles.CreateCotizacionDetalle
{
    public class CreateCotizacionDetalleCommandHandler : IRequestHandler<CreateCotizacionDetalleCommand, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CreateCotizacionDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(CreateCotizacionDetalleCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var cotizacionCode = request.CotizacionCode.Trim().ToUpperInvariant();
            var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Logistica.Transacciones.Cotizaciones.Query()
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.PlantaCode == plantaCode && c.Code == cotizacionCode, cancellationToken);

            if (cotizacion is null)
            {
                throw new KeyNotFoundException($"Cotización {plantaCode}/{cotizacionCode} no encontrada.");
            }

            if (cotizacion.Estado != EstadoCotizacion.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Cotización está en estado '{cotizacion.Estado}' y ya no admite agregar artículos.");
            }

            if (cotizacion.Detalles.Any(d => d.ArticuloCode == articuloCode))
            {
                throw new InvalidOperationException($"El artículo {articuloCode} ya está en esta Cotización.");
            }

            var nextItem = cotizacion.Detalles.Count == 0 ? 1 : cotizacion.Detalles.Max(d => d.ItemNumber) + 1;

            var detalle = new CotizacionDetalle
            {
                PlantaCode = plantaCode,
                CotizacionCode = cotizacionCode,
                ArticuloCode = articuloCode,
                ItemNumber = nextItem
            };

            var pedidoLineasAfectadas = new List<(string, string, int)>();

            if (request.Origenes is { Count: > 0 })
            {
                foreach (var origen in request.Origenes)
                {
                    var pedidoCode = origen.PedidoCode.Trim().ToUpperInvariant();
                    detalle.Origenes.Add(new CotizacionPedidoOrigen
                    {
                        PlantaCode = plantaCode,
                        CotizacionCode = cotizacionCode,
                        ArticuloCode = articuloCode,
                        PedidoCode = pedidoCode,
                        PedidoItemNumber = origen.PedidoItemNumber,
                        CantidadTomada = origen.CantidadTomada
                    });
                    pedidoLineasAfectadas.Add((plantaCode, pedidoCode, origen.PedidoItemNumber));
                }

                detalle.CantidadArticulo = detalle.Origenes.Sum(o => o.CantidadTomada);
            }
            else
            {
                detalle.CantidadArticulo = request.CantidadArticulo ?? 0;
            }

            cotizacion.Detalles.Add(detalle);
            await _uow.Logistica.Transacciones.CotizacionDetalles.AddAsync(detalle, cancellationToken);

            if (pedidoLineasAfectadas.Count > 0)
            {
                await CotizacionCalculators.RecalculatePedidoCantidadCotizadaAsync(_uow, pedidoLineasAfectadas, cancellationToken);
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return CotizacionMapper.Map(cotizacion);
        }
    }
}