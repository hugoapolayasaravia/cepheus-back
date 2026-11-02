namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.Common
{
    public class NoAfectoRetencionResponse
    {
        public string Ruc { get; set; } = default!;
        public string Type { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Resolution { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
