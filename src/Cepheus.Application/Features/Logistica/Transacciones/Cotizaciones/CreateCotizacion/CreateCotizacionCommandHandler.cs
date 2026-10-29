// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/CreateCotizacion/CreateCotizacionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.CreateCotizacion
{
    public class CreateCotizacionCommandHandler : IRequestHandler<CreateCotizacionCommand, CotizacionResponse>
    {
        private const int MaxConcurrencyRetries = 3;
        private const int CodeLength = 6;

        private readonly IUnitOfWork _uow;

        public CreateCotizacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(CreateCotizacionCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await NextCodeAsync(plantaCode, cancellationToken);

                var cotizacion = new Cotizacion
                {
                    PlantaCode = plantaCode,
                    Code = nextCode,
                    FechaLimite = request.FechaLimite,
                    Estado = EstadoCotizacion.Pendiente,
                    Observaciones = request.Observaciones?.Trim() ?? string.Empty
                };

                var item = 1;
                var pedidoLineasAfectadas = new List<(string, string, int)>();

                foreach (var line in request.Detalles)
                {
                    var articuloCode = line.ArticuloCode.Trim().ToUpperInvariant();
                    var detalle = new CotizacionDetalle
                    {
                        PlantaCode = plantaCode,
                        CotizacionCode = nextCode,
                        ArticuloCode = articuloCode,
                        ItemNumber = item++
                    };

                    if (line.Origenes is { Count: > 0 })
                    {
                        foreach (var origen in line.Origenes)
                        {
                            var pedidoCode = origen.PedidoCode.Trim().ToUpperInvariant();
                            detalle.Origenes.Add(new CotizacionPedidoOrigen
                            {
                                PlantaCode = plantaCode,
                                CotizacionCode = nextCode,
                                ArticuloCode = articuloCode,
                                PedidoCode = pedidoCode,
                                PedidoItemNumber = origen.PedidoItemNumber,
                                CantidadTomada = origen.CantidadTomada
                            });
                            pedidoLineasAfectadas.Add((plantaCode, pedidoCode, origen.PedidoItemNumber));
                        }

                        // Confirmado: mientras haya orígenes de Pedido, la cantidad
                        // SIEMPRE es la suma de lo tomado — no se acepta un valor
                        // distinto del cliente.
                        detalle.CantidadArticulo = detalle.Origenes.Sum(o => o.CantidadTomada);
                    }
                    else
                    {
                        // Cotización standalone: la cantidad la define el usuario.
                        detalle.CantidadArticulo = line.CantidadArticulo ?? 0;
                    }

                    cotizacion.Detalles.Add(detalle);
                }

                await _uow.Logistica.Transacciones.Cotizaciones.AddAsync(cotizacion, cancellationToken);

                if (pedidoLineasAfectadas.Count > 0)
                {
                    await CotizacionCalculators.RecalculatePedidoCantidadCotizadaAsync(
                        _uow, pedidoLineasAfectadas, cancellationToken);
                }

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    return CotizacionMapper.Map(cotizacion);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
                    // Colisión de correlativo por creación simultánea en la misma planta.
                }
            }

            throw new InvalidOperationException(
                $"No se pudo generar el correlativo de Cotización para la planta {plantaCode} por alta concurrencia. Intente nuevamente.");
        }

        private async Task<string> NextCodeAsync(string plantaCode, CancellationToken cancellationToken)
        {
            var lastCode = await _uow.Logistica.Transacciones.Cotizaciones.Query()
                .Where(c => c.PlantaCode == plantaCode)
                .OrderByDescending(c => c.Code)
                .Select(c => c.Code)
                .FirstOrDefaultAsync(cancellationToken);

            var next = 1;
            if (lastCode is not null && int.TryParse(lastCode, out var lastNumber))
            {
                next = lastNumber + 1;
            }

            return next.ToString().PadLeft(CodeLength, '0');
        }
    }
}