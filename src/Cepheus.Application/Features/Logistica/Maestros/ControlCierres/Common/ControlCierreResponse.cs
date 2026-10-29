namespace Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common
{
    public class ControlCierreResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string PeriodCode { get; set; } = default!;
        public DateTime ClosureDate { get; set; }
        public decimal DifferenceAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
