using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.DeleteCotizacionMetradoDetalle
{
    public class DeleteCotizacionMetradoDetalleCommandHandler : IRequestHandler<DeleteCotizacionMetradoDetalleCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCotizacionMetradoDetalleCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteCotizacionMetradoDetalleCommand request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();
            var tipoProducto = request.ProductoTipoCode.Trim().ToUpperInvariant();
            var producto = request.ProductoCode.Trim().ToUpperInvariant();
            var orden = request.Order.Trim();

            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstOrDefaultAsync(c => c.NegocioCode == negocio && c.Year == request.Year
                                       && c.Month == request.Month && c.Code == code, cancellationToken);

            if (cotizacion is null)
                throw new KeyNotFoundException($"Cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            if (!cotizacion.IsEditable || cotizacion.Status != EstadoCotizacion.Pendiente)
                throw new InvalidOperationException("Solo se pueden eliminar líneas de metrado de una cotización Pendiente y modificable.");

            var detalle = await _uow.Facturacion.Transacciones.CotizacionesMetradoDetalle.Query()
                .FirstOrDefaultAsync(d => d.NegocioCode == negocio && d.Year == request.Year && d.Month == request.Month
                                       && d.Code == code && d.LevelNumber == request.LevelNumber && d.Order == orden
                                       && d.ProductoTipoCode == tipoProducto && d.ProductoCode == producto,
                                     cancellationToken);

            if (detalle is null)
                throw new KeyNotFoundException(
                    $"Línea {orden}/{tipoProducto}{producto} del nivel {request.LevelNumber} de la cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            _uow.Facturacion.Transacciones.CotizacionesMetradoDetalle.Remove(detalle);
            await _uow.SaveChangesAsync(cancellationToken);

            await CotizacionMetradoResumenRecalculator.RecalculateAsync(
                _uow, negocio, request.Year, request.Month, code, request.LevelNumber, cancellationToken);
        }
    }
}
