// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/CopyCotizacion/CopyCotizacionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.CopyCotizacion
{
    /// <summary>
    /// Copia cabecera + CotizacionDetalle (solo Planta/Artículo/Cantidad),
    /// nace en Pendiente, con fecha/usuario de la copia (CreatedAt/CreatedBy
    /// vía auditoría). Decisión propia (a confirmar si no aplica): NO copia
    /// CotizacionProveedores/DProvCotizacion (es una RFQ nueva, sin ofertas
    /// todavía) ni CotizacionPedidoOrigen (evita duplicar el consumo ya
    /// registrado contra los Pedidos originales — la copia es una lista de
    /// artículos "desprendida" de esos pedidos, no una repetición del
    /// vínculo). OriginalCode queda apuntando a la cotización de origen.
    /// </summary>
    public class CopyCotizacionCommandHandler : IRequestHandler<CopyCotizacionCommand, CotizacionResponse>
    {
        private const int MaxConcurrencyRetries = 3;
        private const int CodeLength = 6;

        private readonly IUnitOfWork _uow;

        public CopyCotizacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(CopyCotizacionCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var sourceCode = request.Code.Trim().ToUpperInvariant();

            var source = await _uow.Logistica.Transacciones.Cotizaciones.Query()
                .AsNoTracking()
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.PlantaCode == plantaCode && c.Code == sourceCode, cancellationToken);

            if (source is null)
            {
                throw new KeyNotFoundException($"Cotización {plantaCode}/{sourceCode} no encontrada.");
            }

            for (var attempt = 1; attempt <= MaxConcurrencyRetries; attempt++)
            {
                var nextCode = await NextCodeAsync(plantaCode, cancellationToken);

                var copia = new Cotizacion
                {
                    PlantaCode = plantaCode,
                    Code = nextCode,
                    FechaLimite = request.NuevaFechaLimite,
                    Estado = EstadoCotizacion.Pendiente,
                    Observaciones = source.Observaciones,
                    OriginalCode = source.Code
                };

                var item = 1;
                foreach (var d in source.Detalles.OrderBy(d => d.ItemNumber))
                {
                    copia.Detalles.Add(new CotizacionDetalle
                    {
                        PlantaCode = plantaCode,
                        CotizacionCode = nextCode,
                        ArticuloCode = d.ArticuloCode,
                        ItemNumber = item++,
                        CantidadArticulo = d.CantidadArticulo
                    });
                }

                await _uow.Logistica.Transacciones.Cotizaciones.AddAsync(copia, cancellationToken);

                try
                {
                    await _uow.SaveChangesAsync(cancellationToken);
                    return CotizacionMapper.Map(copia);
                }
                catch (DbUpdateException) when (attempt < MaxConcurrencyRetries)
                {
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