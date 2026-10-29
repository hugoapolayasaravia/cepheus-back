namespace Cepheus.Application.Features.Comunes.ComprobantesPago.Common
{
    public class ComprobantePagoResponse
    {
        public string Code { get; set; } = default!;
        public string SunatCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string ShortName { get; set; } = default!;
        public string? Description { get; set; }
        public bool RequiresRuc { get; set; }
        public bool RequiresAddress { get; set; }

        public bool AffectsIgv { get; set; }
        public bool IsNonTaxable { get; set; }
        public bool AffectsIncomeTax { get; set; }
        public bool AffectsFonavi { get; set; }
        public bool IsService { get; set; }
        public bool AffectsForeignIgv { get; set; }
        public bool AvailableForPurchaseOrder { get; set; }


        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
