using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.ActualizarComprobanteRetencion
{
    public class ActualizarComprobanteRetencionCommandHandler
        : IRequestHandler<ActualizarComprobanteRetencionCommand, PagoProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public ActualizarComprobanteRetencionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagoProveedorResponse> Handle(
            ActualizarComprobanteRetencionCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var pago = await _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken)
                ?? throw new KeyNotFoundException($"No se puede localizar la operación {code}.");

            if (pago.Estado == EstadoPagoProveedor.Anulado)
            {
                throw new InvalidOperationException("El pago está anulado y no admite cambios.");
            }

            // Legacy: el botón solo abre la ventana si el pago ya tiene comprobante de retención.
            if (string.IsNullOrWhiteSpace(pago.NumeroComprobanteRetencion))
            {
                throw new InvalidOperationException("La operación no tiene comprobante de retención.");
            }

            pago.NumeroComprobanteRetencion = request.NumeroComprobanteRetencion.Trim();
            if (request.FechaComprobanteRetencion.HasValue)
            {
                pago.FechaComprobanteRetencion = request.FechaComprobanteRetencion.Value.Date;
            }

            await _uow.SaveChangesAsync(cancellationToken);
            _uow.ClearTracking();

            return (await PagoProveedorMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
