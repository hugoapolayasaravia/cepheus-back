using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.GenerarAsientoCancelacionVenta
{
    public class GenerarAsientoCancelacionVentaCommandHandler
        : IRequestHandler<GenerarAsientoCancelacionVentaCommand, CancelacionVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public GenerarAsientoCancelacionVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CancelacionVentaResponse> Handle(
            GenerarAsientoCancelacionVentaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var c = await CancelacionVentaRules.GetTrackedAsync(_uow, code, ct);
                    CancelacionVentaRules.EnsureRowVersion(c, request.RowVersion);

                    if (CancelacionVentaRules.HasAsiento(c))
                    {
                        throw new InvalidOperationException($"El asiento de cancelación ya fue generado ({c.ProvisionEntryNumber}).");
                    }

                    if (c.Status != EstadoCancelacion.Pendiente)
                    {
                        throw new InvalidOperationException($"La cancelación {code} está {c.Status}; solo se genera en estado Pendiente.");
                    }

                    // Legacy: no se liquida un cheque diferido antes de su fecha.
                    if (c.TipoPagoCode == CancelacionTipoPago.ChequeDiferido && DateTime.Today < c.Date.Date)
                    {
                        throw new InvalidOperationException(
                            $"Está intentando liquidar un cheque diferido fuera de fecha. Fecha actual: {DateTime.Today:dd/MM/yyyy}, " +
                            $"fecha de cancelación: {c.Date:dd/MM/yyyy}.");
                    }

                    var asiento = await CancelacionVentaAsientoService.BuildAsync(_uow, c, ct);
                    var asientoCode = CancelacionVentaRules.AsientoCode(asiento);

                    c.ProvisionEntryNumber = asientoCode;

                    // Cheques (02 y 07).
                    if (CancelacionTipoPago.GeneraCheque(c.TipoPagoCode))
                    {
                        var cheque = await CancelacionVentaInstrumentoService.FindChequeAsync(_uow, c, ct);
                        if (cheque is not null)
                        {
                            cheque.ProvisionEntryNumber = asientoCode;

                            // El cheque diferido de tipo 02 sigue Pendiente hasta que se liquide; el resto queda Cobrado.
                            if (c.TipoPagoCode == CancelacionTipoPago.ChequeDiferido || !cheque.IsDeferred)
                            {
                                cheque.Status = EstadoCheque.Cobrado;
                            }
                        }
                    }

                    // Letras (03) y Letras Varias (11): con asiento quedan emitidas.
                    if (c.TipoPagoCode is CancelacionTipoPago.Letras or CancelacionTipoPago.LetrasVarias)
                    {
                        var letras = await _uow.Facturacion.Transacciones.LetrasVenta.Query()
                            .Where(l => l.CancelacionCode == c.Code)
                            .ToListAsync(ct);

                        foreach (var letra in letras)
                        {
                            letra.ProvisionEntryNumber = asientoCode;
                            letra.IsPrinted = true;
                        }
                    }

                    await _uow.SaveChangesAsync(ct);

                    // Legacy ue_set_btn_act_cancelacion(codigo, '1'): totales, estado y saldo de los documentos.
                    // En el cheque diferido (07) es aquí donde recién se actualizan los documentos.
                    await CobranzaDocumentoService.ActualizarCancelacionAsync(
                        _uow, c.Code, apply: true, ct, updateDocuments: true);

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
