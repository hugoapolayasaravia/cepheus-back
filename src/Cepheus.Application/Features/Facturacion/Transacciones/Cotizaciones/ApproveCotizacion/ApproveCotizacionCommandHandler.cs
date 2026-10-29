using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.Cotizaciones.ApproveCotizacion
{
    /// <summary>
    /// Solo se puede aprobar una cotización Pendiente. Al aprobar, se llena
    /// ApprovedBy/ApprovedAt y se bloquea la edición del detalle
    /// (IsEditable = false), mismo criterio que ChangeObraEstadoCommandHandler
    /// para campos que se autocompletan según el nuevo estado.
    /// </summary>
    public class ApproveCotizacionCommandHandler : IRequestHandler<ApproveCotizacionCommand, CotizacionResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public ApproveCotizacionCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<CotizacionResponse> Handle(ApproveCotizacionCommand request, CancellationToken cancellationToken)
        {
            var negocio = request.NegocioCode.Trim().ToUpperInvariant();
            var code = request.Code.Trim().ToUpperInvariant();

            var cotizacion = await _uow.Facturacion.Transacciones.Cotizaciones.Query()
                .FirstOrDefaultAsync(c => c.NegocioCode == negocio && c.Year == request.Year
                                       && c.Month == request.Month && c.Code == code, cancellationToken);

            if (cotizacion is null)
                throw new KeyNotFoundException($"Cotización {negocio}/{request.Year}/{request.Month}/{code} no encontrada.");

            if (cotizacion.Status != EstadoCotizacion.Pendiente)
                throw new InvalidOperationException("Solo se puede aprobar una cotización en estado Pendiente.");

            var hasDetail = await _uow.Facturacion.Transacciones.CotizacionesDetalle.Query()
                .AnyAsync(d => d.NegocioCode == negocio && d.Year == request.Year
                            && d.Month == request.Month && d.Code == code, cancellationToken);

            if (!hasDetail)
                throw new InvalidOperationException("No se puede aprobar una cotización sin líneas de detalle.");

            cotizacion.Status = EstadoCotizacion.Aprobado;
            cotizacion.ApprovedBy = _currentUser.FullName;
            cotizacion.ApprovedAt = DateTime.UtcNow;
            cotizacion.IsEditable = false;

            await _uow.SaveChangesAsync(cancellationToken);

            return CotizacionMapper.Map(cotizacion);
        }
    }
}
