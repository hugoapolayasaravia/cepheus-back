using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.ControlesVentas.Common;
using Cepheus.Domain.Comunes;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.ControlesVentas.UpdateControlVentas
{
    public class UpdateControlVentasCommandHandler : IRequestHandler<UpdateControlVentasCommand, ControlVentasResponse>
    {
        private readonly IUnitOfWork _uow;

        public UpdateControlVentasCommandHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ControlVentasResponse> Handle(UpdateControlVentasCommand request, CancellationToken cancellationToken)
        {
            var current = await _uow.Comunes.ControlesVentas.Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

            if (current is null)
            {
                throw new KeyNotFoundException($"Configuración de Controles de Ventas {request.Id} no encontrada.");
            }

            var control = new ControlVentas
            {
                Id = request.Id,
                IgvPercentage = request.IgvPercentage,
                WithholdingPercentage = request.WithholdingPercentage,
                ClosingPeriod = request.ClosingPeriod.Trim(),
                SalesProcessDate = request.SalesProcessDate,
                PurchasesProcessDate = request.PurchasesProcessDate,
                SalesCancelDate = request.SalesCancelDate,
                PurchasesCancelDate = request.PurchasesCancelDate,
                WithholdingCap = request.WithholdingCap,
                DetractionPercentage = request.DetractionPercentage,
                IncomeTaxPercentage = request.IncomeTaxPercentage,
                FonaviPercentage = request.FonaviPercentage,
                ForeignIgvPercentage = request.ForeignIgvPercentage,
                QuotaPercentage = request.QuotaPercentage,
                BlocksGrouping = request.BlocksGrouping,
                WithholdingCapInvoice = request.WithholdingCapInvoice,
                WorkOrderDaysLimit = request.WorkOrderDaysLimit,
                WorkOrderMaxDays = request.WorkOrderMaxDays,
                SaleMaxDays = request.SaleMaxDays,
                BalanceLimit = request.BalanceLimit,
                AdditionalActivationDays = request.AdditionalActivationDays,
                IsServiceIndicator = request.IsServiceIndicator,

                CreatedAt = current.CreatedAt,
                CreatedBy = current.CreatedBy,

                RowVersion = request.RowVersion
            };

            _uow.Comunes.ControlesVentas.Update(control);

            try
            {
                await _uow.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new InvalidOperationException(
                    "La configuración fue modificada por otro proceso. Recargue los datos e intente nuevamente.");
            }

            return new ControlVentasResponse
            {
                Id = control.Id,
                IgvPercentage = control.IgvPercentage,
                WithholdingPercentage = control.WithholdingPercentage,
                ClosingPeriod = control.ClosingPeriod,
                SalesProcessDate = control.SalesProcessDate,
                PurchasesProcessDate = control.PurchasesProcessDate,
                SalesCancelDate = control.SalesCancelDate,
                PurchasesCancelDate = control.PurchasesCancelDate,
                WithholdingCap = control.WithholdingCap,
                DetractionPercentage = control.DetractionPercentage,
                IncomeTaxPercentage = control.IncomeTaxPercentage,
                FonaviPercentage = control.FonaviPercentage,
                ForeignIgvPercentage = control.ForeignIgvPercentage,
                QuotaPercentage = control.QuotaPercentage,
                BlocksGrouping = control.BlocksGrouping,
                WithholdingCapInvoice = control.WithholdingCapInvoice,
                WorkOrderDaysLimit = control.WorkOrderDaysLimit,
                WorkOrderMaxDays = control.WorkOrderMaxDays,
                SaleMaxDays = control.SaleMaxDays,
                BalanceLimit = control.BalanceLimit,
                AdditionalActivationDays = control.AdditionalActivationDays,
                IsServiceIndicator = control.IsServiceIndicator,
                CreatedAt = control.CreatedAt,
                UpdatedAt = control.UpdatedAt,
                CreatedBy = control.CreatedBy,
                UpdatedBy = control.UpdatedBy,
                RowVersion = control.RowVersion
            };
        }
    }
}
