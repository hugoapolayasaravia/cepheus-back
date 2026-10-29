namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorRemuneracions.Common
{
    public class TrabajadorRemuneracionResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public decimal SueldoBasico { get; set; }
        public string? MonedaCode { get; set; }
        public string? ModoPagoCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
