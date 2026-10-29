namespace Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common
{
    public class ArticuloStockResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string ArticuloCode { get; set; } = default!;
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal UnitCostUsd { get; set; }
        public decimal? AverageCost { get; set; }
        public decimal MinStock { get; set; }
        public decimal MaxStock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
