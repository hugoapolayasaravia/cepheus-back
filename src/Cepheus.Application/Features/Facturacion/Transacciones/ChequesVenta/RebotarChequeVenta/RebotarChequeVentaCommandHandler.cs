using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.ChequesVenta.RebotarChequeVenta
{
    public class RebotarChequeVentaCommandHandler : IRequestHandler<RebotarChequeVentaCommand, ChequeVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public RebotarChequeVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ChequeVentaResponse> Handle(RebotarChequeVentaCommand request, CancellationToken cancellationToken)
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

                    if (!ChequeVentaRules.CanRebotar(cheque))
                    {
                        throw new InvalidOperationException(
                            $"Solo se pueden rebotar cheques Cobrados o Pendientes. Estado actual: {cheque.Status}.");
                    }

                    if (!ChequeVentaRules.HasCancelacionEntry(cheque))
                    {
                        throw new InvalidOperationException(
                            "El cheque no tiene el asiento de la cancelación que lo originó (13 caracteres).");
                    }

                    var reboteDate = request.ReboteDate.Date;

                    // Legacy fg_cierre(fecha_mch): la fecha del rebote no puede estar en un período cerrado.
                    await ChequeVentaRules.EnsurePeriodOpenAsync(_uow, reboteDate, ct);

                    if (!cheque.IsDeferred)
                    {
                        var asiento = await ChequeVentaReboteAsientoService.BuildAsync(_uow, cheque, reboteDate, ct);
                        cheque.ReboteEntryNumber = asiento.Codigo;
                    }

                    cheque.MotivoReboteCode = request.MotivoReboteCode.Trim();
                    cheque.ReboteDate = reboteDate;
                    cheque.Status = EstadoCheque.Rebotado;

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
