using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.ChangeCotizacionEstado
{
    public class ChangeCotizacionEstadoCommandHandler : IRequestHandler<ChangeCotizacionEstadoCommand, CotizacionResponse>
    {
        private static readonly EstadoCotizacion[] EstadosPermitidos =
        {
            EstadoCotizacion.Pendiente,
            EstadoCotizacion.Aceptado,
            EstadoCotizacion.Proyectado,
            EstadoCotizacion.NoAceptado
        };

        private readonly IUnitOfWork _uow;

        public ChangeCotizacionEstadoCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CotizacionResponse> Handle(ChangeCotizacionEstadoCommand request, CancellationToken cancellationToken)
        {
            if (!System.Enum.TryParse<EstadoCotizacion>(request.NuevoEstado, true, out var nuevoEstado)
                || !EstadosPermitidos.Contains(nuevoEstado))
            {
                throw new ArgumentException(
                    $"Estado '{request.NuevoEstado}' no válido para este comando. Use uno de: {string.Join(", ", EstadosPermitidos)}. " +
                    "Para Aprobado use ApproveCotizacionCommand; para Anulado use AnularCotizacionCommand.");
            }

            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstOrDefaultAsync(c => c.NegocioCode == negocio && c.Year == request.Year
                                       && c.Month == request.Month && c.Code == code, cancellationToken);

            if (cotizacion is null)
                throw new KeyNotFoundException($"Cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            if (cotizacion.Status == EstadoCotizacion.Anulado)
                throw new InvalidOperationException("Una cotización anulada no puede cambiar de estado.");

            cotizacion.Status = nuevoEstado;
            cotizacion.IsEditable = nuevoEstado == EstadoCotizacion.Pendiente;

            await _uow.SaveChangesAsync(cancellationToken);

            return CotizacionMapper.Map(cotizacion);
        }
    }
}
