namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorSeguros.Common
{
    public class TrabajadorSeguroResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string? EpsCode { get; set; }
        public string? SituacionEpsCode { get; set; }
        public string? NumeroSeguro { get; set; }
        public string? SctrTipoCode { get; set; }
        public string? SctrSaludCode { get; set; }
        public string? SctrPensionCode { get; set; }
        public bool EpsActivo { get; set; }
        public bool SeguroMedico { get; set; }
        public bool EssaludVida { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
