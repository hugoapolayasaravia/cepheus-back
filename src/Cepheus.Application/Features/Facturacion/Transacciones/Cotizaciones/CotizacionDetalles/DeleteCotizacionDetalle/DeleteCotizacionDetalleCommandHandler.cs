using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.DeleteCotizacionDetalle
{
    public class DeleteCotizacionDetalleCommandHandler : IRequestHandler<DeleteCotizacionDetalleCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCotizacionDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteCotizacionDetalleCommand request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstOrDefaultAsync(c => c.NegocioCode == negocio && c.Year == request.Year
                                       && c.Month == request.Month && c.Code == code, cancellationToken);

            if (cotizacion is null)
                throw new KeyNotFoundException($"Cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            if (!cotizacion.IsEditable || cotizacion.Status != EstadoCotizacion.Pendiente)
                throw new InvalidOperationException("Solo se pueden eliminar líneas de una cotización Pendiente y modificable.");

            var detalle = await _uow.Facturacion.Transacciones.CotizacionesDetalle.Query()
                .FirstOrDefaultAsync(d => d.NegocioCode == negocio && d.Year == request.Year
                                       && d.Month == request.Month && d.Code == code && d.Item == request.Item,
                                     cancellationToken);

            if (detalle is null)
                throw new KeyNotFoundException($"Línea {request.Item} de la cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            _uow.Facturacion.Transacciones.CotizacionesDetalle.Remove(detalle);
            await _uow.SaveChangesAsync(cancellationToken);

            await CotizacionTotalesRecalculator.RecalculateAsync(_uow, negocio, request.Year, request.Month, code, cancellationToken);
        }
    }
}
