namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.ManoObra.Common
{
    public class PlanPreventivoManoObraResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string PlanCode { get; set; } = default!;
        public string TrabajadorCode { get; set; } = default!;
        public string TrabajadorName { get; set; } = default!;
        public decimal Tiempo { get; set; }
        public decimal CostoTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
