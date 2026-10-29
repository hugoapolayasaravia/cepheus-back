namespace Cepheus.Application.Features.Facturacion.Maestros.Obras.Common
{
    public class ObraResponse
    {
        public string ClienteCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string UbigeoCode { get; set; } = default!;
        public string? Observations { get; set; }
        public string Estado { get; set; } = default!;
        public string? DeliveryAddress { get; set; }
        public string DeliveryUbigeoCode { get; set; } = default!;
        public string BillingAddress { get; set; } = default!;
        public string BillingUbigeoCode { get; set; } = default!;
        public string? ResponsibleName { get; set; }
        public string ResponsiblePhone { get; set; } = default!;
        public string ResponsibleEmail { get; set; } = default!;
        public string? FormaPagoVentaCode { get; set; }
        public string CobradorCode { get; set; } = default!;
        public string VendedorCode { get; set; } = default!;
        public string? AnalisisVentaCode { get; set; }
        public decimal CreditLimit { get; set; }
        public string CreditCurrencyCode { get; set; } = default!;
        public DateTime? EntryDate { get; set; }
        public bool HasSurcharge { get; set; }
        public string ShortName { get; set; } = default!;
        public string? TipoValorizacionCode { get; set; }
        public bool? RequiresValorizacion { get; set; }
        public string? ScheduledWeekday { get; set; }
        public bool IsProject { get; set; }
        public bool RequiresPrinting { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? CompletionUser { get; set; }
        public bool? RequiresManagementApproval { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
