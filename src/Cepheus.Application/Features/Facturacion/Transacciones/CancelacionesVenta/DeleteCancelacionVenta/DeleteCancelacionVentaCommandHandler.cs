using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.DeleteCancelacionVenta
{
    public class DeleteCancelacionVentaCommandHandler : IRequestHandler<DeleteCancelacionVentaCommand, Unit>
    {
        private readonly IUnitOfWork _uow;

        public DeleteCancelacionVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Unit> Handle(DeleteCancelacionVentaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var c = await CancelacionVentaRules.GetTrackedAsync(_uow, code, ct);

                if (CancelacionVentaRules.HasAsiento(c))
                {
                    throw new InvalidOperationException("La cancelación tiene asiento: anule primero el asiento.");
                }

                if (c.Status != EstadoCancelacion.Pendiente)
                {
                    throw new InvalidOperationException($"En estado de operación ({c.Status}) no se puede eliminar.");
                }

                if (c.Details.Count > 0)
                {
                    throw new InvalidOperationException("Operación presenta documentos: retírelos antes de eliminar.");
                }

                await CancelacionVentaInstrumentoService.DeleteAsync(_uow, c, ct);

                _uow.Facturacion.Transacciones.CancelacionesVenta.Remove(c);

                await _uow.SaveChangesAsync(ct);

                return true;
            }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);

            _uow.ClearTracking();

            return Unit.Value;
        }
    }
}
