using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Maestros.Proveedores.Common
{
    public class ProveedorResponse
    {
        public string Code { get; set; } = default!;
        public string DocumentTypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string LegalName { get; set; } = default!;
        public string? TradeName { get; set; }
        public ProviderType ProviderType { get; set; }
        public ProviderOrigin Origin { get; set; }
        public SunatCondition? SunatCondition { get; set; }
        public SunatTaxpayerStatus? SunatStatus { get; set; }
        public string? Observations { get; set; }
        public bool IsActive { get; set; }
        public DateTime? DeactivatedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
