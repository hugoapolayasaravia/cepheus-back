namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.Common
{
    public class ConductorResponse
    {
        public string Code { get; set; } = default!;
        public string DocumentTypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string DriverLicenseNumber { get; set; } = default!;
        public string? LicenseCategory { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Observations { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
