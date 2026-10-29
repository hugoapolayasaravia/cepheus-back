using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.AnularCotizacion
{
    /// <summary>
    /// Anulado es terminal: cualquier estado no-Anulado puede anularse
    /// (mismo criterio abierto que ChangeObraEstadoCommandHandler, el legacy
    /// no documenta restricciones de transición), pero una vez Anulado no
    /// se permite ningún otro cambio de estado.
    /// </summary>
    public class AnularCotizacionCommandHandler : IRequestHandler<AnularCotizacionCommand, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public AnularCotizacionCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<CotizacionResponse> Handle(AnularCotizacionCommand request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstOrDefaultAsync(c => c.NegocioCode == negocio && c.Year == request.Year
                                       && c.Month == request.Month && c.Code == code, cancellationToken);

            if (cotizacion is null)
                throw new KeyNotFoundException($"Cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            if (cotizacion.Status == EstadoCotizacion.Anulado)
                throw new InvalidOperationException("La cotización ya se encuentra anulada.");

            cotizacion.Status = EstadoCotizacion.Anulado;
            cotizacion.CancelReason = request.Reason.Trim();
            cotizacion.CanceledBy = _currentUser.FullName;
            cotizacion.CanceledAt = DateTime.UtcNow;
            cotizacion.IsEditable = false;

            await _uow.SaveChangesAsync(cancellationToken);

            return CotizacionMapper.Map(cotizacion);
        }
    }
}
