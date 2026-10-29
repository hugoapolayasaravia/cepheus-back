namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSaluds.Common
{
    public class TrabajadorSaludResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string? TipoSangreCode { get; set; }
        public string? AlergiaCode { get; set; }
        public string? Otros { get; set; }
        public DateTime? FechaEvaluacionMedica { get; set; }
        public string? Observaciones { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
