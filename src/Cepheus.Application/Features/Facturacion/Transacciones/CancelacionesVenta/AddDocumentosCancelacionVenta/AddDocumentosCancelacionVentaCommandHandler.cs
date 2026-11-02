using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.CancelacionesVenta.AddDocumentosCancelacionVenta
{
    public class AddDocumentosCancelacionVentaCommandHandler
        : IRequestHandler<AddDocumentosCancelacionVentaCommand, CancelacionVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public AddDocumentosCancelacionVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<CancelacionVentaResponse> Handle(
            AddDocumentosCancelacionVentaCommand request, CancellationToken cancellationToken)
        {
            var code = request.Code.Trim();

            try
            {
                await _uow.ExecuteInTransactionAsync(async ct =>
                {
                    var c = await CancelacionVentaRules.GetTrackedAsync(_uow, code, ct);
                    CancelacionVentaRules.EnsureRowVersion(c, request.RowVersion);
                    CancelacionVentaRules.EnsureEditable(c);
                    CancelacionVentaRules.EnsureDocumentsEditable(c);

                    await CancelacionVentaDocumentosService.AddAsync(_uow, c, request.Documents, ct);
                    await CancelacionVentaRules.EnsureWithinTotalAsync(_uow, c, ct);

                    await _uow.SaveChangesAsync(ct);

                    // Legacy: ad_actualizadocumentos('0') de cada documento y ad_actualizacancelaciones('0').
                    await CancelacionVentaRules.RecalculateAsync(_uow, c, apply: false, ct);
                    await _uow.SaveChangesAsync(ct);

                    return true;
                }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La cancelación o alguno de los documentos fue modificado por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await CancelacionVentaMapper.LoadAsync(_uow, code, cancellationToken))!;
        }
    }
}
