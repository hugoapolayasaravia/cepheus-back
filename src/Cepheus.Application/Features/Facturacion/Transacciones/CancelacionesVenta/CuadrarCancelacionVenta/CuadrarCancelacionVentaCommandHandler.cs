using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.CuadrarCancelacionVenta
{
    public class CuadrarCancelacionVentaCommandHandler
        : IRequestHandler<CuadrarCancelacionVentaCommand, CancelacionVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public CuadrarCancelacionVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CancelacionVentaResponse> Handle(
            CuadrarCancelacionVentaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();
            var item = request.Item.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var c = await CancelacionVentaRules.GetTrackedAsync(_uow, code, ct);
                    CancelacionVentaRules.EnsureRowVersion(c, request.RowVersion);
                    CancelacionVentaRules.EnsureEditable(c);
                    CancelacionVentaRules.EnsureDocumentsEditable(c);

                    if (c.Details.Count == 0)
                    {
                        throw new InvalidOperationException("La cancelación no tiene documentos.");
                    }

                    var line = c.Details.FirstOrDefault(d => d.Item == item)
                        ?? throw new KeyNotFoundException($"La cancelación {code} no tiene el documento ítem {item}.");

                    // Saldo al día antes de cuadrar.
                    await CancelacionVentaRules.RecalculateAsync(_uow, c, apply: false, ct);

                    if (c.TipoPagoCode != CancelacionTipoPago.Canje)
                    {
                        if (c.AvailableAmount == 0m)
                        {
                            throw new InvalidOperationException("La cancelación no tiene diferencia por cuadrar.");
                        }

                        line.DifferenceSoles += AsientoVentaService.ToSoles(c.AvailableAmount, c.MonedaCode, c.ExchangeRate);

                        await _uow.SaveChangesAsync(ct);
                        await CancelacionVentaRules.RecalculateAsync(_uow, c, apply: false, ct);
                    }
                    else
                    {
                        // Legacy: en el canje se dejan en cero los totales de la cancelación.
                        c.TotalAmount = 0m;
                        c.UsedAmount = 0m;
                        c.DifferenceAmount = 0m;
                        c.AvailableAmount = 0m;
                    }

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
