namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OTRMateriales.Common
{
    public class OTRMaterialResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string OrdenTrabajoCode { get; set; } = default!;
        public DateTime FechaProceso { get; set; }
        public string ArticuloCode { get; set; } = default!;
        public decimal Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal CostoTotal { get; set; }
        public string? EstadoCode { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
