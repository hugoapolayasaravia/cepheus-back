using Cepheus.Application.Features.Comunes.ControlesVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.ControlesVentas.UpdateControlVentas
{
    public record UpdateControlVentasCommand(
         int Id,
         decimal IgvPercentage,
         decimal WithholdingPercentage,
         string ClosingPeriod,
         DateTime SalesProcessDate,
         DateTime PurchasesProcessDate,
         DateTime SalesCancelDate,
         DateTime PurchasesCancelDate,
         decimal WithholdingCap,
         decimal DetractionPercentage,
         decimal IncomeTaxPercentage,
         decimal FonaviPercentage,
         decimal ForeignIgvPercentage,
         decimal QuotaPercentage,
         bool BlocksGrouping,
         decimal WithholdingCapInvoice,
         int WorkOrderDaysLimit,
         int WorkOrderMaxDays,
         int SaleMaxDays,
         int? BalanceLimit,
         int? AdditionalActivationDays,
         bool IsServiceIndicator,
         byte[] RowVersion
     ) : IRequest<ControlVentasResponse>;
}
