namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSindicatos.Common
{
    public class TrabajadorSindicatoResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public bool Afiliado { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
