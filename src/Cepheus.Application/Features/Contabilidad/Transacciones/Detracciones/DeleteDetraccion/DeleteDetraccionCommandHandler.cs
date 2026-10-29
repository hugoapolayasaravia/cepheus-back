using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.DeleteDetraccion
{
    /// <summary>
    /// Elimina un lote Pendiente (legacy ue_set_btn_eli_bor). Sus documentos quedan libres para otro lote.
    /// </summary>
    public class DeleteDetraccionCommandHandler : IRequestHandler<DeleteDetraccionCommand, Unit>
    {
        private readonly IUnitOfWork _uow;

        public DeleteDetraccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<Unit> Handle(DeleteDetraccionCommand request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);

            var detraccion = await _uow.Contabilidad.Transacciones.Detracciones.Query()
                .Include(d => d.Items)
                .ThenInclude(i => i.Documents)
                .FirstOrDefaultAsync(d => d.Year == year && d.Period == period && d.LotNumber == lot, cancellationToken);

            if (detraccion is null)
            {
                throw new KeyNotFoundException(DetraccionRules.NotFoundMessage(year, period, lot));
            }

            DetraccionRules.EnsurePendiente(detraccion, "eliminar");

            // El legacy bloqueaba la eliminación si algún documento tenía cheque de tesorería (DCheCompras).
            // TODO: validar también contra cheques cuando se suba la entidad de Tesorería.
            var hasPayments = await _uow.Contabilidad.Transacciones.DetraccionDocumentos.Query()
                .AnyAsync(x => x.Year == year && x.Period == period && x.LotNumber == lot
                               && (x.DocumentoCompra.PagadoSol > 0m
                                   || x.DocumentoCompra.Estado == EstadoDocumentoCompra.Cancelado),
                    cancellationToken);

            if (hasPayments)
            {
                throw new InvalidOperationException(
                    "Uno o más documentos del lote ya fueron cancelados por tesorería: no se puede eliminar.");
            }

            foreach (var item in detraccion.Items.ToList())
            {
                foreach (var document in item.Documents.ToList())
                {
                    _uow.Contabilidad.Transacciones.DetraccionDocumentos.Remove(document);
                }

                _uow.Contabilidad.Transacciones.DetraccionItems.Remove(item);
            }

            _uow.Contabilidad.Transacciones.Detracciones.Remove(detraccion);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La detracción fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return Unit.Value;
        }
    }
}
