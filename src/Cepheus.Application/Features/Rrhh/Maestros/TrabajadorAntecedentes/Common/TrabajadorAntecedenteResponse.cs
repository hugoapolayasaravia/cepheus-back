namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorAntecedentes.Common
{
    public class TrabajadorAntecedenteResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public bool TieneAntecedentes { get; set; }
        public string? Descripcion { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
