namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMaquinas.Common
{
    public class OTRMaquinaResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string OrdenTrabajoCode { get; set; } = default!;
        public string MaquinaCode { get; set; } = default!;
        public DateTime FechaProceso { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Horas { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
