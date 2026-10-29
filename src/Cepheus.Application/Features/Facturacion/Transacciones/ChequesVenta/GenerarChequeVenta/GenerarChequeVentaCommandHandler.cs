using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.GenerarChequeVenta
{
    public class GenerarChequeVentaCommandHandler : IRequestHandler<GenerarChequeVentaCommand, ChequeVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GenerarChequeVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChequeVentaResponse> Handle(GenerarChequeVentaCommand request, CancellationToken cancellationToken)
        {
            var cliente = request.ClienteCode.Trim();
            var banco = ChequeVentaRules.Normalize(request.BancoCode);
            var number = request.Number.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var cheque = await ChequeVentaRules.GetTrackedAsync(_uow, cliente, banco, number, ct);

                    ChequeVentaRules.EnsureRowVersion(cheque, request.RowVersion);

                    if (!ChequeVentaRules.CanGenerar(cheque))
                    {
                        throw new InvalidOperationException(
                            "Solo se liquidan cheques diferidos en estado Pendiente.");
                    }

                    if (!ChequeVentaRules.HasCancelacionEntry(cheque))
                    {
                        throw new InvalidOperationException(
                            "El cheque no tiene el asiento de la cancelación que lo originó (13 caracteres).");
                    }

                    var operationDate = request.OperationDate.Date;

                    await ChequeVentaRules.EnsurePeriodOpenAsync(_uow, operationDate, ct);

                    cheque.OperationNumber = request.OperationNumber.Trim();
                    cheque.OperationDate = operationDate;
                    cheque.Status = EstadoCheque.Cobrado;

                    await _uow.SaveChangesAsync(ct);

                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "El cheque fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await ChequeVentaMapper.LoadAsync(_uow, cliente, banco, number, cancellationToken))!;
        }
    }
}
