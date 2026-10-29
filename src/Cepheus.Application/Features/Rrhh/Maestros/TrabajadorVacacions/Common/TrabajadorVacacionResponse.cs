namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorVacacions.Common
{
    public class TrabajadorVacacionResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public DateTime? FechaVacaciones { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
