namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloProveedores.Common
{
    public class ArticuloProveedorResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string ArticuloCode { get; set; } = default!;
        public string ProveedorCode { get; set; } = default!;
        public bool IsAgreement { get; set; }
        public decimal? AgreementPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
