namespace Cepheus.Application.Features.Comunes.Monedas.Common
{
    public class MonedaResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Symbol { get; set; }
        public string? NumericCode { get; set; }
        public int DecimalPlaces { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
