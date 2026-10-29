namespace Cepheus.Application.Features.Mantenimiento.Maestros.Actividades.Common
{
    public class ActividadResponse
    {
        public string Code { get; set; } = default!;
        public string VerboActividadCode { get; set; } = default!;
        public string VerboActividadName { get; set; } = default!;
        public string ObjetoActividadCode { get; set; } = default!;
        public string ObjetoActividadName { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
