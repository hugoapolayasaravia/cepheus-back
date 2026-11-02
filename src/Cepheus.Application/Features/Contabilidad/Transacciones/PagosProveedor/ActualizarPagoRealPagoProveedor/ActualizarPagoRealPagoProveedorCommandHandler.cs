using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.PagosProveedor.ActualizarPagoRealPagoProveedor
{
    public class ActualizarPagoRealPagoProveedorCommandHandler
        : IRequestHandler<ActualizarPagoRealPagoProveedorCommand, PagoProveedorResponse>
    {
        private readonly IUnitOfWork _uow;

        public ActualizarPagoRealPagoProveedorCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PagoProveedorResponse> Handle(
            ActualizarPagoRealPagoProveedorCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            var pago = await _uow.Contabilidad.Transacciones.PagosProveedor.Query()
                .FirstOrDefaultAsync(p => p.Code == code, cancellationToken)
                ?? throw new KeyNotFoundException($"Pago {code} no encontrado.");

            if (pago.Estado == EstadoPagoProveedor.Anulado)
            {
                throw new InvalidOperationException("El pago está anulado y no admite cambios.");
            }

            pago.PagoRealSol = request.PagoRealSol;

            await _uow.SaveChangesAsync(cancellationToken);
            _uow.ClearTracking();

            return (await PagoProveedorMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
