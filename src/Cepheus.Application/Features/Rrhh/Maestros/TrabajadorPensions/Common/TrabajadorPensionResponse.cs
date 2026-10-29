namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorPensions.Common
{
    public class TrabajadorPensionResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string? TipoAfiliacionCode { get; set; }
        public string? AfpCode { get; set; }
        public DateTime? FechaAfiliacion { get; set; }
        public string? NumeroAfp { get; set; }
        public string? RegimenPensionarioCode { get; set; }
        public string? TipoPensionCode { get; set; }
        public string? NumeroCarnetSsp { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
