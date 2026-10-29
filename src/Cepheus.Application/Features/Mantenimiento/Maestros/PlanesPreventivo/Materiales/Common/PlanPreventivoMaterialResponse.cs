namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Materiales.Common
{
    public class PlanPreventivoMaterialResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string PlanCode { get; set; } = default!;
        public string ArticuloCode { get; set; } = default!;
        public string ArticuloName { get; set; } = default!;
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal CostoTotal { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
