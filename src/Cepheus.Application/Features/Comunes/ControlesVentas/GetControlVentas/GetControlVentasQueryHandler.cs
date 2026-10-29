using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Comunes.ControlesVentas.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Comunes.ControlesVentas.GetControlVentas
{
    public class GetControlVentasQueryHandler : IRequestHandler<GetControlVentasQuery, ControlVentasResponse>
    {
        private readonly IUnitOfWork _uow;

        public GetControlVentasQueryHandler(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ControlVentasResponse> Handle(GetControlVentasQuery request, CancellationToken cancellationToken)
        {
            var control = await _uow.Comunes.ControlesVentas.Query()
                .AsNoTracking()
                .Select(c => new ControlVentasResponse
                {
                    Id = c.Id,
                    IgvPercentage = c.IgvPercentage,
                    WithholdingPercentage = c.WithholdingPercentage,
                    ClosingPeriod = c.ClosingPeriod,
                    SalesProcessDate = c.SalesProcessDate,
                    PurchasesProcessDate = c.PurchasesProcessDate,
                    SalesCancelDate = c.SalesCancelDate,
                    PurchasesCancelDate = c.PurchasesCancelDate,
                    WithholdingCap = c.WithholdingCap,
                    DetractionPercentage = c.DetractionPercentage,
                    IncomeTaxPercentage = c.IncomeTaxPercentage,
                    FonaviPercentage = c.FonaviPercentage,
                    ForeignIgvPercentage = c.ForeignIgvPercentage,
                    QuotaPercentage = c.QuotaPercentage,
                    BlocksGrouping = c.BlocksGrouping,
                    WithholdingCapInvoice = c.WithholdingCapInvoice,
                    WorkOrderDaysLimit = c.WorkOrderDaysLimit,
                    WorkOrderMaxDays = c.WorkOrderMaxDays,
                    SaleMaxDays = c.SaleMaxDays,
                    BalanceLimit = c.BalanceLimit,
                    AdditionalActivationDays = c.AdditionalActivationDays,
                    IsServiceIndicator = c.IsServiceIndicator,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    CreatedBy = c.CreatedBy,
                    UpdatedBy = c.UpdatedBy,
                    RowVersion = c.RowVersion
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (control is null)
            {
                throw new KeyNotFoundException(
                    "No existe configuración de Controles de Ventas. Debe sembrarse un registro inicial.");
            }

            return control;
        }
    }
}
