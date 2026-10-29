namespace Cepheus.Application.Features.Comunes.TiposCambio.Common
{
    public class TipoCambioResponse
    {
        public int Id { get; set; }
        public string MonedaCode { get; set; } = default!;
        public string? MonedaName { get; set; }
        public DateOnly Date { get; set; }
        public decimal SellRate { get; set; }
        public decimal BuyRate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
