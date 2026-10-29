namespace Cepheus.Application.Features.Mantenimiento.Maestros.ActividadesMantenimiento.Common
{
    public class ActividadMantenimientoResponse
    {
        public string ActividadCode { get; set; } = default!;
        public string ActividadName { get; set; } = default!;
        public string ActividadCubiertaCode { get; set; } = default!;
        public string ActividadCubiertaName { get; set; } = default!;
        public int Prioridad { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
