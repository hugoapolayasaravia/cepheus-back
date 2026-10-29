namespace Cepheus.Application.Features.Facturacion.Catalogos.AlturasLosa.Common
{
    public class AlturaLosaResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public decimal Value { get; set; }
        public decimal Width { get; set; }
        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public string? PolystyreneProductoTipoCode { get; set; }
        public string? PolystyreneProductoCode { get; set; }
        public decimal PolystyreneValue { get; set; }
        public decimal PolystyreneWidth { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
