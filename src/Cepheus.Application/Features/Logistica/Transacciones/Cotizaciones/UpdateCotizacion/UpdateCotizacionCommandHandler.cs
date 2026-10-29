// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/UpdateCotizacion/UpdateCotizacionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.UpdateCotizacion
{
    /// <summary>
    /// Igual criterio que UpdatePedidoCommandHandler: solo edita cabecera
    /// (FechaLimite, Observaciones) y solo mientras Estado == Pendiente. El
    /// detalle y los proveedores se gestionan por sus propios CRUD.
    /// </summary>
    public class UpdateCotizacionCommandHandler : IRequestHandler<UpdateCotizacionCommand, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCotizacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(UpdateCotizacionCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Logistica.Transacciones.Cotizaciones.Query()
                .Include(c => c.Detalles).ThenInclude(d => d.Origenes)
                .Include(c => c.Proveedores).ThenInclude(p => p.Detalles)
                .FirstOrDefaultAsync(c => c.PlantaCode == plantaCode && c.Code == code, cancellationToken);

            if (cotizacion is null)
            {
                throw new KeyNotFoundException($"Cotización {plantaCode}/{code} no encontrada.");
            }

            if (cotizacion.Estado != EstadoCotizacion.Pendiente)
            {
                throw new InvalidOperationException(
                    $"La Cotización está en estado '{cotizacion.Estado}' y ya no admite edición de cabecera.");
            }

            cotizacion.FechaLimite = request.FechaLimite;
            cotizacion.Observaciones = request.Observaciones?.Trim() ?? string.Empty;
            cotizacion.RowVersion = request.RowVersion;

            _uow.Logistica.Transacciones.Cotizaciones.Update(cotizacion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La Cotización fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return CotizacionMapper.Map(cotizacion);
        }
    }
}