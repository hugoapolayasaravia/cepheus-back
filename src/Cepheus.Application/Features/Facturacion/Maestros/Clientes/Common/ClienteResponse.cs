namespace Cepheus.Application.Features.Facturacion.Maestros.Clientes.Common
{
    public class ClienteResponse
    {
        public string Code { get; set; } = default!;
        public string PersonType { get; set; } = default!;
        public string DocumentTypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string? Name { get; set; }
        public string Address { get; set; } = default!;
        public string UbigeoCode { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? ParentClientCode { get; set; }
        public string TipoClienteCode { get; set; } = default!;
        public string ClasificacionClienteCode { get; set; } = default!;
        public string? LegalRepresentativeName { get; set; }
        public string? LegalRepresentativePhone { get; set; }
        public string? LegalRepresentativeDni { get; set; }
        public string ContactName { get; set; } = default!;
        public string ContactPhone { get; set; } = default!;
        public string ContactEmail { get; set; } = default!;
        public string? Observations { get; set; }
        public bool IsVip { get; set; }
        public bool RequiresCashOnly { get; set; }
        public bool HasGlobalCreditLine { get; set; }
        public decimal? GlobalCreditAmount { get; set; }
        public bool? RequiresPurchaseOrderApproval { get; set; }
        public bool? RequiresWorkOrderApproval { get; set; }
        public string FormaPagoVentaCode { get; set; } = default!;
        public string? CurrencyCode { get; set; }
        public bool? RequiresManagementApproval { get; set; }
        public string Estado { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
