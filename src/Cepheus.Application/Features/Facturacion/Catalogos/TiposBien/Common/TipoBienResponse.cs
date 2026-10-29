namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposBien.Common
{
    public class TipoBienResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal DetractionRate { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
