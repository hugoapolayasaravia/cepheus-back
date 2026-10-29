namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorBeneficios.Common
{
    public class TrabajadorBeneficioResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public bool Cts { get; set; }
        public bool Gratificacion { get; set; }
        public bool Vacaciones { get; set; }
        public bool MovilidadAntesEntrada { get; set; }
        public bool MovilidadDespuesSalida { get; set; }
        public bool Refrigerio { get; set; }
        public bool Cena { get; set; }
        public bool Vale { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
