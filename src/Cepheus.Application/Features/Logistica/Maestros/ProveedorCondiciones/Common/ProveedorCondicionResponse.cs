namespace Cepheus.Application.Features.Logistica.Maestros.ProveedorCondiciones.Common
{
    public class ProveedorCondicionResponse
    {
        public int Id { get; set; }
        public string ProveedorCode { get; set; } = default!;
        public string FormaPagoCode { get; set; } = default!;
        public int PaymentTermDays { get; set; }
        public string MonedaCode { get; set; } = default!;
        public decimal? CreditLimit { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
