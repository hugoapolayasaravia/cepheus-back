using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.DeleteDetraccionItem
{
    /// <summary>
    /// Legacy: SP COM_SP_DETRACCION_ITEM_DELETE. Cambio: el total del lote se recalcula siempre
    /// (el legacy no lo actualizaba al quitar el último ítem) y solo se permite en un lote Pendiente.
    /// </summary>
    public class DeleteDetraccionItemCommandHandler : IRequestHandler<DeleteDetraccionItemCommand, DetraccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public DeleteDetraccionItemCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DetraccionResponse> Handle(DeleteDetraccionItemCommand request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);
            var proveedor = request.ProveedorCode.Trim();

            var detraccion = await _uow.Contabilidad.Transacciones.Detracciones.Query()
                .Include(d => d.Items)
                .ThenInclude(i => i.Documents)
                .FirstOrDefaultAsync(d => d.Year == year && d.Period == period && d.LotNumber == lot, cancellationToken);

            if (detraccion is null)
            {
                throw new KeyNotFoundException(DetraccionRules.NotFoundMessage(year, period, lot));
            }

            DetraccionRules.EnsurePendiente(detraccion, "quitar ítems de");

            var item = detraccion.Items.FirstOrDefault(i => i.ProveedorCode == proveedor && i.ItemNumber == request.ItemNumber);

            if (item is null)
            {
                throw new KeyNotFoundException(
                    $"El ítem {request.ItemNumber} del proveedor {proveedor} no existe en la detracción {DetraccionRules.Label(year, period, lot)}.");
            }

            foreach (var document in item.Documents.ToList())
            {
                _uow.Contabilidad.Transacciones.DetraccionDocumentos.Remove(document);
            }

            _uow.Contabilidad.Transacciones.DetraccionItems.Remove(item);
            detraccion.Items.Remove(item);

            DetraccionRules.Recalculate(detraccion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _uow.ClearTracking();
                throw new InvalidOperationException(
                    "La detracción fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await DetraccionMapper.LoadAsync(_uow, year, period, lot, cancellationToken))!;
        }
    }
}
