using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.DeleteCotizacionMetradoResumen
{
    /// <summary>
    /// Eliminar un nivel elimina en cascada su detalle de metrado
    /// (CotizacionMetradoDetalle), ver CotizacionMetradoResumenConfiguration
    /// (FK con DeleteBehavior.Cascade hacia el detalle).
    /// </summary>
    public class DeleteCotizacionMetradoResumenCommandHandler : IRequestHandler<DeleteCotizacionMetradoResumenCommand>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCotizacionMetradoResumenCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task Handle(DeleteCotizacionMetradoResumenCommand request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstOrDefaultAsync(c => c.NegocioCode == negocio && c.Year == request.Year
                                       && c.Month == request.Month && c.Code == code, cancellationToken);

            if (cotizacion is null)
                throw new KeyNotFoundException($"Cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            if (!cotizacion.IsEditable || cotizacion.Status != EstadoCotizacion.Pendiente)
                throw new InvalidOperationException("Solo se pueden eliminar niveles de metrado de una cotización Pendiente y modificable.");

            var resumen = await _uow.Facturacion.Transacciones.CotizacionesMetradoResumen.Query()
                .FirstOrDefaultAsync(r => r.NegocioCode == negocio && r.Year == request.Year && r.Month == request.Month
                                       && r.Code == code && r.LevelNumber == request.LevelNumber, cancellationToken);

            if (resumen is null)
                throw new KeyNotFoundException($"Nivel {request.LevelNumber} de la cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrado.");

            _uow.Facturacion.Transacciones.CotizacionesMetradoResumen.Remove(resumen);
            await _uow.SaveChangesAsync(cancellationToken);
        }
    }
}
