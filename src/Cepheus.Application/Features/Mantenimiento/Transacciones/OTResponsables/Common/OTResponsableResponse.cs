namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTResponsables.Common
{
    public class OTResponsableResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string OrdenTrabajoCode { get; set; } = default!;
        public DateTime FechaProceso { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public decimal TiempoProceso { get; set; }
        public decimal Basico { get; set; }
        public decimal CostoTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
