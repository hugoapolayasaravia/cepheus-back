namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDocumentos.Common
{
    public class TrabajadorDocumentoResponse
    {
        public int Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string TipoDocumentoCode { get; set; } = default!;
        public string DocumentNumber { get; set; } = default!;
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}