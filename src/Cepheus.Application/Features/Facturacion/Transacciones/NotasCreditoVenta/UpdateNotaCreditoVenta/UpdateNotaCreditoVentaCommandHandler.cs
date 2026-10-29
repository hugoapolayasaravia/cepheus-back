using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.UpdateNotaCreditoVenta
{
    public class UpdateNotaCreditoVentaCommandHandler
        : IRequestHandler<UpdateNotaCreditoVentaCommand, NotaCreditoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNotaCreditoVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaCreditoVentaResponse> Handle(
            UpdateNotaCreditoVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaCreditoVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            var nota = await _uow.Facturacion.Transacciones.NotasCreditoVenta.Query()
                .FirstOrDefaultAsync(n => n.PlantaCode == plantaCode && n.Number == number, cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException(
                    $"Nota de crédito {NotaCreditoVentaRules.FormatKey(plantaCode, number)} no encontrada.");
            }

            if (!nota.RowVersion.SequenceEqual(request.RowVersion))
            {
                throw new InvalidOperationException(
                    "La nota de crédito fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            if (nota.Status != EstadoNotaCredito.Pendiente)
            {
                throw new InvalidOperationException("Solo se puede modificar una nota de crédito Pendiente.");
            }

            if (request.IssueDate.Date != nota.IssueDate.Date)
            {
                var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(_uow, request.IssueDate, cancellationToken);
                var exchangeRate = await DocumentoVentaRules.ResolveExchangeRateAsync(
                    _uow, nota.MonedaCode, request.IssueDate, cancellationToken);

                var totals = DocumentoVentaRules.ComputeTotals(nota.SubTotal, igvPercentage);

                nota.IssueDate = request.IssueDate;
                nota.ExchangeRate = exchangeRate;
                nota.IgvAmount = totals.IgvAmount;
                nota.TotalAmount = totals.TotalAmount;
                nota.PendingAmount = totals.TotalAmount;
            }

            nota.Description = request.Description.Trim();

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La nota de crédito fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await NotaCreditoVentaMapper.LoadAsync(_uow, plantaCode, number, cancellationToken))!;
        }
    }
}
