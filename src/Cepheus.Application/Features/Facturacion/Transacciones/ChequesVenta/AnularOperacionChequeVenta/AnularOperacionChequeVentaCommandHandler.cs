using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.AnularOperacionChequeVenta
{
    public class AnularOperacionChequeVentaCommandHandler
        : IRequestHandler<AnularOperacionChequeVentaCommand, ChequeVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public AnularOperacionChequeVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChequeVentaResponse> Handle(
            AnularOperacionChequeVentaCommand request, CancellationToken cancellationToken)
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

                    if (!ChequeVentaRules.HasCancelacionEntry(cheque))
                    {
                        throw new InvalidOperationException(
                            "El cheque no tiene el asiento de la cancelación que lo originó (13 caracteres).");
                    }

                    if (cheque.Status == EstadoCheque.Rebotado && cheque.PendingAmount != cheque.TotalAmount)
                    {
                        throw new InvalidOperationException(
                            "El cheque rebotado ya fue utilizado en otra operación: anule primero esa cancelación.");
                    }

                    if (!ChequeVentaRules.CanAnularOperacion(cheque))
                    {
                        throw new InvalidOperationException(
                            "El cheque no tiene una operación que se pueda deshacer.");
                    }

                    if (cheque.Status == EstadoCheque.Rebotado)
                    {
                        // Deshacer el rebote.
                        if (!string.IsNullOrWhiteSpace(cheque.ReboteEntryNumber))
                        {
                            await AsientoAnulacionService.AnularAsync(_uow, cheque.ReboteEntryNumber.Trim(), ct);
                        }

                        cheque.ReboteEntryNumber = null;
                        cheque.MotivoReboteCode = null;
                        cheque.ReboteDate = null;

                        // Vuelve al estado previo al rebote: un cheque diferido sin liquidar estaba Pendiente.
                        cheque.Status = cheque.IsDeferred && string.IsNullOrWhiteSpace(cheque.OperationNumber)
                            ? EstadoCheque.Pendiente
                            : EstadoCheque.Cobrado;
                    }
                    else
                    {
                        // Deshacer la liquidación de un cheque diferido.
                        cheque.OperationNumber = null;
                        cheque.OperationDate = null;
                        cheque.Status = EstadoCheque.Pendiente;
                    }

                    cheque.Description = null;

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
