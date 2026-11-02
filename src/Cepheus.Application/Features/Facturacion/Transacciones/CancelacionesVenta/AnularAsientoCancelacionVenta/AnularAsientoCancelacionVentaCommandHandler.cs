using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.AsientosContables.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.AnularAsientoCancelacionVenta
{
    public class AnularAsientoCancelacionVentaCommandHandler
        : IRequestHandler<AnularAsientoCancelacionVentaCommand, CancelacionVentaResponse>
    {
        private readonly IUnitOfWork _uow;
        private readonly ICurrentUserService _currentUser;

        public AnularAsientoCancelacionVentaCommandHandler(IUnitOfWork uow, ICurrentUserService currentUser)
        {
            _uow = uow;
            _currentUser = currentUser;
        }

        public async Task<CancelacionVentaResponse> Handle(
            AnularAsientoCancelacionVentaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var c = await CancelacionVentaRules.GetTrackedAsync(_uow, code, ct);
                    CancelacionVentaRules.EnsureRowVersion(c, request.RowVersion);

                    if (!CancelacionVentaRules.HasAsiento(c))
                    {
                        throw new InvalidOperationException($"La cancelación {code} no tiene asiento generado.");
                    }

                    if (c.Status == EstadoCancelacion.Anulado)
                    {
                        throw new InvalidOperationException($"La cancelación {code} ya está anulada.");
                    }

                    // Legacy fg_cierre: mes y año cerrados contablemente.
                    await CancelacionVentaRules.EnsurePeriodOpenAsync(_uow, c.Date, ct);

                    var asientoCode = c.ProvisionEntryNumber!.Trim();

                    // ---- Validaciones por tipo de pago, antes de tocar nada.
                    var cheque = CancelacionTipoPago.GeneraCheque(c.TipoPagoCode)
                        ? await CancelacionVentaInstrumentoService.FindChequeAsync(_uow, c, ct)
                        : null;

                    if (CancelacionTipoPago.GeneraCheque(c.TipoPagoCode))
                    {
                        if (cheque is null)
                        {
                            throw new InvalidOperationException("Cheque no está registrado.");
                        }

                        var esperado = c.TipoPagoCode == CancelacionTipoPago.Cheque && cheque.IsDeferred
                            ? EstadoCheque.Pendiente
                            : EstadoCheque.Cobrado;

                        if (cheque.Status != esperado)
                        {
                            throw new InvalidOperationException(
                                esperado == EstadoCheque.Pendiente
                                    ? "El cheque es diferido y no se encuentra Pendiente."
                                    : $"El cheque no se encuentra Cobrado (estado {cheque.Status}).");
                        }
                    }

                    var letras = c.TipoPagoCode is CancelacionTipoPago.Letras or CancelacionTipoPago.LetrasVarias
                        ? await _uow.Facturacion.Transacciones.LetrasVenta.Query()
                            .Where(l => l.CancelacionCode == c.Code).ToListAsync(ct)
                        : new List<Cepheus.Domain.Facturacion.Transacciones.LetraVenta>();

                    if (letras.Any(l => l.Status == EstadoLetra.Cancelado || l.PendingAmount != l.TotalAmount))
                    {
                        throw new InvalidOperationException(
                            "Las letras de la operación tienen cancelaciones aplicadas: anule primero esas cancelaciones.");
                    }

                    // ---- Anula el asiento (legacy fc_anulaasiento).
                    await AsientoAnulacionService.AnularAsync(_uow, asientoCode, ct);

                    c.ProvisionEntryNumber = null;
                    c.Status = EstadoCancelacion.Pendiente;

                    if (cheque is not null)
                    {
                        cheque.ProvisionEntryNumber = null;

                        // El cheque diferido (07) nació Pendiente: al deshacer la liquidación vuelve a serlo.
                        if (c.TipoPagoCode == CancelacionTipoPago.ChequeDiferido)
                        {
                            cheque.Status = EstadoCheque.Pendiente;
                        }
                    }

                    foreach (var letra in letras)
                    {
                        letra.ProvisionEntryNumber = null;
                        letra.IsPrinted = false;
                    }

                    // ---- Letras Varias: se retiran los documentos y la operación queda Anulada.
                    if (c.TipoPagoCode == CancelacionTipoPago.LetrasVarias)
                    {
                        var freed = new List<(string Type, string Planta, string Number, string? Banco)>();

                        foreach (var line in c.Details.ToList())
                        {
                            var doc = await CobranzaDocumentoService.LoadAsync(
                                _uow, line.TypeCode, line.PlantaCode, line.DocumentNumber, line.ClienteCode,
                                line.DocumentBancoCode, ct);
                            doc?.SetCashDocument?.Invoke(null);

                            freed.Add((line.TypeCode, line.PlantaCode, line.DocumentNumber, line.DocumentBancoCode));

                            c.Details.Remove(line);
                            _uow.Facturacion.Transacciones.CancelacionesVentaDetalle.Remove(line);
                        }

                        c.Status = EstadoCancelacion.Anulado;
                        c.CanceledBy = _currentUser.FullName;
                        c.CanceledAt = DateTime.UtcNow;
                        c.CancelReason = request.Reason.Trim();

                        foreach (var letra in letras)
                        {
                            letra.Status = EstadoLetra.Anulado;
                            letra.PendingAmount = 0m;
                            letra.CanceledAt = c.CanceledAt;
                        }

                        await _uow.SaveChangesAsync(ct);

                        // Legacy ad_actualizadocumentos('1') de cada documento retirado.
                        foreach (var f in freed)
                        {
                            await CobranzaDocumentoService.ActualizarAsync(
                                _uow, f.Type, f.Planta, f.Number, c.ClienteCode, f.Banco, apply: true, c.Date, ct);
                        }

                        await CobranzaDocumentoService.ActualizarCancelacionAsync(
                            _uow, c.Code, apply: false, ct, updateDocuments: false);
                    }

                    await _uow.SaveChangesAsync(ct);

                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La cancelación o uno de sus documentos fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await CancelacionVentaMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
