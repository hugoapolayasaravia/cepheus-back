// Cepheus.Application/Features/Logistica/Transacciones/Cotizaciones/ChangeEstadoCotizacion/ChangeEstadoCotizacionCommandHandler.cs
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Cotizaciones.ChangeEstadoCotizacion
{
    public class ChangeEstadoCotizacionCommandHandler : IRequestHandler<ChangeEstadoCotizacionCommand, CotizacionResponse>
    {
        private static readonly Dictionary<EstadoCotizacion, EstadoCotizacion[]> ValidTransitions = new()
        {
            [EstadoCotizacion.Pendiente] = new[] { EstadoCotizacion.EnEvaluacion, EstadoCotizacion.Anulado },
            [EstadoCotizacion.EnEvaluacion] = new[] { EstadoCotizacion.Cerrado, EstadoCotizacion.Anulado },
            [EstadoCotizacion.Cerrado] = Array.Empty<EstadoCotizacion>(),
            [EstadoCotizacion.Anulado] = Array.Empty<EstadoCotizacion>()
        };

        private readonly IUnitOfWork _uow;

        public ChangeEstadoCotizacionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(ChangeEstadoCotizacionCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = request.PlantaCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            if (!System.Enum.TryParse<EstadoCotizacion>(request.NuevoEstado, true, out var nuevoEstado))
            {
                throw new ArgumentException($"Estado '{request.NuevoEstado}' no es válido.");
            }

            var cotizacion = await _uow.Logistica.Transacciones.Cotizaciones.Query()
                .Include(c => c.Detalles).ThenInclude(d => d.Origenes)
                .Include(c => c.Proveedores).ThenInclude(p => p.Detalles)
                .FirstOrDefaultAsync(c => c.PlantaCode == plantaCode && c.Code == code, cancellationToken);

            if (cotizacion is null)
            {
                throw new KeyNotFoundException($"Cotización {plantaCode}/{code} no encontrada.");
            }

            if (!ValidTransitions[cotizacion.Estado].Contains(nuevoEstado))
            {
                throw new InvalidOperationException(
                    $"No se puede pasar de '{cotizacion.Estado}' a '{nuevoEstado}'. " +
                    $"Transiciones válidas: {string.Join(", ", ValidTransitions[cotizacion.Estado])}.");
            }

            // Regla de negocio: para Cerrar la cotización debe haber al menos un
            // proveedor Seleccionado — es el paso previo a generar la OC.
            if (nuevoEstado == EstadoCotizacion.Cerrado &&
                !cotizacion.Proveedores.Any(p => p.Estado == EstadoProveedorCotizacion.Seleccionado))
            {
                throw new InvalidOperationException(
                    "No se puede cerrar la Cotización sin al menos un proveedor Seleccionado.");
            }

            cotizacion.Estado = nuevoEstado;

            if (nuevoEstado == EstadoCotizacion.Cerrado)
            {
                cotizacion.FechaCierre = DateTime.UtcNow;
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return CotizacionMapper.Map(cotizacion);
        }
    }
}