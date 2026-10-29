namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContratos.Common
{
    public class TrabajadorContratoResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string? TipoContratoCode { get; set; }
        public string? TipoExtensionCode { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime? FechaTermino { get; set; }
        public bool? Renovado { get; set; }
        public string? TipoDuracion { get; set; }
        public int? CantidadDuracion { get; set; }
        public bool Activo { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
