using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.RemoveDocumentoCancelacionVenta
{
    public class RemoveDocumentoCancelacionVentaCommandHandler
        : IRequestHandler<RemoveDocumentoCancelacionVentaCommand, CancelacionVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public RemoveDocumentoCancelacionVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CancelacionVentaResponse> Handle(
            RemoveDocumentoCancelacionVentaCommand request, CancellationToken cancellationToken)
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

                    var line = c.Details.FirstOrDefault(d => d.Item == item)
                        ?? throw new KeyNotFoundException($"La cancelación {code} no tiene el documento ítem {item}.");

                    var (type, planta, number, banco) = (line.TypeCode, line.PlantaCode, line.DocumentNumber, line.DocumentBancoCode);

                    c.Details.Remove(line);
                    _uow.Facturacion.Transacciones.CancelacionesVentaDetalle.Remove(line);

                    await _uow.SaveChangesAsync(ct);

                    // Legacy: ad_actualizadocumentos('1') del documento retirado y ad_actualizacancelaciones('0').
                    // En el cheque diferido (07) el legacy no actualiza el documento; aquí sí, para que nunca quede
                    // aplicado un documento que ya no está en la cancelación.
                    await CobranzaDocumentoService.ActualizarAsync(
                        _uow, type, planta, number, c.ClienteCode, banco, apply: true, c.Date, ct);

                    await CancelacionVentaRules.RecalculateAsync(_uow, c, apply: false, ct);

                    await _uow.SaveChangesAsync(ct);

                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La cancelación o el documento fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await CancelacionVentaMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
