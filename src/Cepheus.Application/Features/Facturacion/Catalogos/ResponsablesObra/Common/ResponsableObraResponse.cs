namespace Cepheus.Application.Features.Facturacion.Catalogos.ResponsablesObra.Common
{
    public class ResponsableObraResponse
    {
        public string ClienteCode { get; set; } = default!;
        public string ObraCode { get; set; } = default!;
        public int Code { get; set; }
        public string Name { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Position { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
