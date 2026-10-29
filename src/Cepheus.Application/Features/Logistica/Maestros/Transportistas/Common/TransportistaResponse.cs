namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.Common
{
    public class TransportistaResponse
    {
        public string Code { get; set; } = default!;
        public string DocumentTypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string LegalName { get; set; } = default!;
        public string? TradeName { get; set; }
        public string? Address { get; set; }
        public string? UbigeoCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? MtcRegistrationNumber { get; set; }
        public string? CertificationNumber { get; set; }
        public bool IsOwnFleet { get; set; }
        public string? Observations { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
