namespace Cepheus.Application.Features.Facturacion.Catalogos.Fletes.Common
{
    public class FleteResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Amount { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
