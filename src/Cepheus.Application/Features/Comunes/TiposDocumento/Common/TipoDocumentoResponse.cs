namespace Cepheus.Application.Features.Comunes.TiposDocumento.Common
{
    public class TipoDocumentoResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? ShortName { get; set; }
        public string? SunatCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
