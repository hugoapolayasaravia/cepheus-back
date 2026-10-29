namespace Cepheus.Application.Features.Facturacion.Maestros.Transportistas.Common
{
    public class TransportistaResponse
    {
        public string Code { get; set; } = default!;
        public string DocumentTypeCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public string? UbigeoCode { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? MtcInternalCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
