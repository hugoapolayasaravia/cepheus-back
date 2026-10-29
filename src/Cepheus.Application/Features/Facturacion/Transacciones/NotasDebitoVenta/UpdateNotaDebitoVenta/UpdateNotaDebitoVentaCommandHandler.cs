using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.PeriodosIgv.Common;
using Cepheus.Application.Features.Facturacion.Common;
using Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common;
using Cepheus.Domain.Facturacion.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.UpdateNotaDebitoVenta
{
    public class UpdateNotaDebitoVentaCommandHandler
        : IRequestHandler<UpdateNotaDebitoVentaCommand, NotaDebitoVentaResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateNotaDebitoVentaCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<NotaDebitoVentaResponse> Handle(
            UpdateNotaDebitoVentaCommand request, CancellationToken cancellationToken)
        {
            var plantaCode = NotaDebitoVentaRules.Normalize(request.PlantaCode);
            var number = request.Number.Trim();

            var nota = await _uow.Facturacion.Transacciones.NotasDebitoVenta.Query()
                .Include(n => n.Details)
                .FirstOrDefaultAsync(n => n.PlantaCode == plantaCode && n.Number == number, cancellationToken);

            if (nota is null)
            {
                throw new KeyNotFoundException(
                    $"Nota de débito {NotaDebitoVentaRules.FormatKey(plantaCode, number)} no encontrada.");
            }

            if (!nota.RowVersion.SequenceEqual(request.RowVersion))
            {
                throw new InvalidOperationException(
                    "La nota de débito fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            if (nota.Status != EstadoNotaDebito.Pendiente)
            {
                throw new InvalidOperationException("Solo se puede modificar una nota de débito Pendiente.");
            }

            if (nota.IsPrinted)
            {
                throw new InvalidOperationException("El documento se encuentra impreso.");
            }

            var igvPercentage = await PeriodoIgvRules.GetPorcentajeAsync(_uow, request.IssueDate, cancellationToken);
            var exchangeRate = await DocumentoVentaRules.ResolveExchangeRateAsync(
                _uow, nota.MonedaCode, request.IssueDate, cancellationToken);

            var productNames = await NotaDebitoVentaRules.LoadProductNamesAsync(_uow, request.Details, cancellationToken);
            var (fresh, subTotal) = NotaDebitoVentaRules.BuildDetails(plantaCode, number, request.Details, productNames);
            var totals = DocumentoVentaRules.ComputeTotals(subTotal, igvPercentage);

            NotaDebitoVentaRules.ReplaceDetails(nota, fresh);

            nota.IssueDate = request.IssueDate;
            nota.ExchangeRate = exchangeRate;
            nota.Description = request.Description.Trim();
            nota.SubTotal = totals.SubTotal;
            nota.IgvAmount = totals.IgvAmount;
            nota.TotalAmount = totals.TotalAmount;
            nota.PendingAmount = totals.TotalAmount;

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La nota de débito fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            _uow.ClearTracking();

            return (await NotaDebitoVentaMapper.LoadAsync(_uow, plantaCode, number, cancellationToken))!;
        }
    }
}
