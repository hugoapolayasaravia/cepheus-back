namespace Cepheus.Application.Features.Logistica.Maestros.RangosAprobacion.Common
{
    public class RangoAprobacionResponse
    {
        public string NivelCode { get; set; } = default!;
        public string TipoTransaccionCode { get; set; } = default!;
        public string UnidadNegocioCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public decimal ImporteMinimo { get; set; }
        public decimal ImporteMaximo { get; set; }
        public decimal ImporteAcumuladoDiario { get; set; }
        public decimal ImporteAcumuladoMensual { get; set; }
        public decimal? PorcentajeTotal { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
