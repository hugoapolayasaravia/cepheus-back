namespace Cepheus.Application.Features.Facturacion.Maestros.Vendedores.Common
{
    public class VendedorResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Abbreviation { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Title { get; set; }
        public int? UserId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
