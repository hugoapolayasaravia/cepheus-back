using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.Common;
using Cepheus.Domain.Contabilidad.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Contabilidad.Transacciones.Detracciones.CancelarDetraccion
{
    public class CancelarDetraccionCommandHandler : IRequestHandler<CancelarDetraccionCommand, DetraccionResponse>
    {
        private readonly IUnitOfWork _uow;

        public CancelarDetraccionCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<DetraccionResponse> Handle(CancelarDetraccionCommand request, CancellationToken cancellationToken)
        {
            var year = DetraccionRules.Normalize(request.Year);
            var period = DetraccionRules.Normalize(request.Period);
            var lot = DetraccionRules.Normalize(request.LotNumber);

            var detraccion = await _uow.Contabilidad.Transacciones.Detracciones.Query()
                .FirstOrDefaultAsync(d => d.Year == year && d.Period == period && d.LotNumber == lot, cancellationToken);

            if (detraccion is null)
            {
                throw new KeyNotFoundException(DetraccionRules.NotFoundMessage(year, period, lot));
            }

            if (detraccion.Status != EstadoDetraccion.Emitido)
            {
                throw new InvalidOperationException(
                    $"Solo se puede cancelar un lote Emitido (con el archivo de depósito generado). El lote está {detraccion.Status}.");
            }

            var cancellationDate = request.CancellationDate.Date;

            if (detraccion.DepositDate.HasValue && cancellationDate < detraccion.DepositDate.Value.Date)
            {
                throw new InvalidOperationException(
                    "La fecha de cancelación no puede ser anterior a la fecha de depósito del lote.");
            }

            detraccion.CancellationDate = cancellationDate;
            detraccion.PaymentReceiptNumber = request.PaymentReceiptNumber.Trim();
            detraccion.Status = EstadoDetraccion.Cancelado;

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

            return (await DetraccionMapper.LoadAsync(_uow, year, period, lot, cancellationToken))!;
        }
    }
}
