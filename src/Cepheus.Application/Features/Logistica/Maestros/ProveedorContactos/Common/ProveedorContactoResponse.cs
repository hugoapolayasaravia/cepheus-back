namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorContactos.Common
{
    public class ProveedorContactoResponse
    {
        public int Id { get; set; }
        public string ProveedorCode { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string? LastName { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string? MobilePhone { get; set; }
        public string? Email { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
