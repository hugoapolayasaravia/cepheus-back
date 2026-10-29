namespace Cepheus.Application.Features.Comunes.ControlesVentas.Common
{
    public class ControlVentasResponse
    {
        public int Id { get; set; }
        public decimal IgvPercentage { get; set; }
        public decimal WithholdingPercentage { get; set; }
        public string ClosingPeriod { get; set; } = default!;
        public DateTime SalesProcessDate { get; set; }
        public DateTime PurchasesProcessDate { get; set; }
        public DateTime SalesCancelDate { get; set; }
        public DateTime PurchasesCancelDate { get; set; }
        public decimal WithholdingCap { get; set; }
        public decimal DetractionPercentage { get; set; }
        public decimal IncomeTaxPercentage { get; set; }
        public decimal FonaviPercentage { get; set; }
        public decimal ForeignIgvPercentage { get; set; }
        public decimal QuotaPercentage { get; set; }
        public bool BlocksGrouping { get; set; }
        public decimal WithholdingCapInvoice { get; set; }
        public int WorkOrderDaysLimit { get; set; }
        public int WorkOrderMaxDays { get; set; }
        public int SaleMaxDays { get; set; }
        public int? BalanceLimit { get; set; }
        public int? AdditionalActivationDays { get; set; }
        public bool IsServiceIndicator { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
