namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.Common
{
    public class TipoAnexoResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Flag { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
