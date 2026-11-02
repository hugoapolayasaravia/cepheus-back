using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.LetrasVenta.EliminarDocumentoLetraVenta
{
    public class EliminarDocumentoLetraVentaCommandHandler
        : IRequestHandler<EliminarDocumentoLetraVentaCommand, LetraVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public EliminarDocumentoLetraVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<LetraVentaResponse> Handle(
            EliminarDocumentoLetraVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = LetraVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();
            var item = request.Item.Trim();

            await _uow.ExecuteInTransactionAsync(async ct =>
            {
                var letra = await LetraVentaRules.GetTrackedAsync(_uow, plantaCode, number, ct);

                if (letra.AcceptedByClient)
                {
                    throw new InvalidOperationException("La letra ya fue aceptada por el cliente: no se pueden eliminar documentos.");
                }

                var line = await _uow.Facturacion.Transacciones.CancelacionesVentaDetalle.Query()
                    .FirstOrDefaultAsync(d => d.CancelacionCode == letra.CancelacionCode && d.Item == item, ct)
                    ?? throw new KeyNotFoundException($"El item {item} no existe en la cancelación {letra.CancelacionCode}.");

                // Libera el documento (legacy: Documento_Caj = '').
                var doc = await CobranzaDocumentoService.LoadAsync(
                    _uow, line.TypeCode, line.PlantaCode, line.DocumentNumber, line.ClienteCode, line.DocumentBancoCode, ct);
                doc?.SetCashDocument?.Invoke(null);

                _uow.Facturacion.Transacciones.CancelacionesVentaDetalle.Remove(line);

                await _uow.SaveChangesAsync(ct);

                return true;
            }, System.Data.IsolationLevel.ReadCommitted, cancellationToken);

            _uow.ClearTracking();

            return (await LetraVentaMapper.LoadAsync(_uow, plantaCode, number, cancellationToken))!;
        }
    }
}
