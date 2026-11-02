using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.UpdateCancelacionVenta
{
    public class UpdateCancelacionVentaCommandHandler
        : IRequestHandler<UpdateCancelacionVentaCommand, CancelacionVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateCancelacionVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CancelacionVentaResponse> Handle(
            UpdateCancelacionVentaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();
            var cobrador = request.CobradorCode.Trim();

            if (!await _uow.Facturacion.Maestros.Cobradores.Query().AnyAsync(c => c.Code == cobrador && c.IsActive, cancellationToken))
                throw new InvalidOperationException($"El cobrador {cobrador} no existe o está inactivo.");

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var c = await CancelacionVentaRules.GetTrackedAsync(_uow, code, ct);
                    CancelacionVentaRules.EnsureRowVersion(c, request.RowVersion);
                    CancelacionVentaRules.EnsureEditable(c);

                    var isCanje = c.TipoPagoCode == CancelacionTipoPago.Canje;
                    var hasLines = c.Details.Count > 0;

                    if ((request.Date.HasValue && request.Date.Value.Date != c.Date.Date) ||
                        (request.TotalAmount.HasValue && DocumentoVentaRules.Round2(request.TotalAmount.Value) != c.TotalAmount))
                    {
                        if (hasLines)
                        {
                            throw new InvalidOperationException(
                                "La fecha y el importe solo se modifican mientras la cancelación no tenga documentos; retírelos primero.");
                        }

                        if (c.TipoPagoCode == CancelacionTipoPago.ChequeDiferido && request.Date.HasValue)
                        {
                            throw new InvalidOperationException(
                                "En el cheque diferido la fecha es la fecha diferida; modifique la fecha diferida.");
                        }

                        if (isCanje && request.TotalAmount.HasValue)
                        {
                            throw new InvalidOperationException("El canje no tiene importe.");
                        }

                        if (request.Date.HasValue && request.Date.Value.Date != c.Date.Date)
                        {
                            var date = request.Date.Value.Date;

                            if (date > DateTime.Today)
                                throw new InvalidOperationException("La fecha de cancelación no puede ser mayor a la fecha de proceso.");

                            await CancelacionVentaRules.EnsurePeriodOpenAsync(_uow, date, ct);

                            c.ExchangeRate = await DocumentoVentaRules.ResolveExchangeRateAsync(_uow, c.MonedaCode, date, ct);
                            c.Date = date;
                        }

                        if (request.TotalAmount.HasValue)
                        {
                            c.TotalAmount = DocumentoVentaRules.Round2(request.TotalAmount.Value);
                            c.AvailableAmount = c.TotalAmount;
                        }
                    }

                    c.CobradorCode = cobrador;
                    c.Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim();

                    if (!isCanje)
                    {
                        var bancoDep = string.IsNullOrWhiteSpace(request.BancoDepCode)
                            ? c.BancoDepCode : request.BancoDepCode.Trim().ToUpperInvariant();

                        if (bancoDep is null)
                            throw new InvalidOperationException("Indique el banco de depósito.");

                        if (!bancoDep.All(ch => ch == '0') &&
                            !await _uow.Comunes.Bancos.Query().AnyAsync(b => b.Code == bancoDep, ct))
                            throw new InvalidOperationException($"El banco de depósito {bancoDep} no existe.");

                        c.BancoDepCode = bancoDep;
                        c.AccountNumber = string.IsNullOrWhiteSpace(request.AccountNumber) ? c.AccountNumber : request.AccountNumber.Trim();
                    }

                    if (c.TipoPagoCode == CancelacionTipoPago.ChequeDiferido || CancelacionTipoPago.GeneraLetra(c.TipoPagoCode) || c.IsDeferred)
                    {
                        if (request.DeferredDate.HasValue)
                        {
                            c.DeferredDate = request.DeferredDate.Value.Date;

                            // Legacy: la fecha del cheque diferido es su fecha de presentación.
                            if (c.TipoPagoCode == CancelacionTipoPago.ChequeDiferido && c.DeferredDate.Value != c.Date.Date)
                            {
                                if (hasLines)
                                    throw new InvalidOperationException("Retire los documentos antes de cambiar la fecha diferida.");

                                await CancelacionVentaRules.EnsurePeriodOpenAsync(_uow, c.DeferredDate.Value, ct);
                                c.Date = c.DeferredDate.Value;
                                c.ExchangeRate = await DocumentoVentaRules.ResolveExchangeRateAsync(_uow, c.MonedaCode, c.Date, ct);
                            }
                        }
                    }

                    if (CancelacionTipoPago.GeneraCheque(c.TipoPagoCode) && !c.IsDeferred)
                    {
                        c.OperationNumber = string.IsNullOrWhiteSpace(request.OperationNumber) ? null : request.OperationNumber.Trim().ToUpperInvariant();
                        c.OperationDate = request.OperationDate?.Date;
                    }
                    else if (c.TipoPagoCode == CancelacionTipoPago.ChequeDiferido && request.OperationDate.HasValue)
                    {
                        c.OperationDate = request.OperationDate.Value.Date;
                    }
                    else if (!CancelacionTipoPago.GeneraCheque(c.TipoPagoCode) && !CancelacionTipoPago.GeneraLetra(c.TipoPagoCode))
                    {
                        c.OperationNumber = string.IsNullOrWhiteSpace(request.OperationNumber) ? null : request.OperationNumber.Trim().ToUpperInvariant();
                        c.OperationDate = request.OperationDate?.Date;
                    }

                    await CancelacionVentaInstrumentoService.SyncAsync(_uow, c, ct);

                    await _uow.SaveChangesAsync(ct);
                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La cancelación fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await CancelacionVentaMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
