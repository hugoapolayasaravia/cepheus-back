namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorFormacions.Common
{
    public class TrabajadorFormacionResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string? NivelEducativoCode { get; set; }
        public string? GradoInstruccionCode { get; set; }
        public string? TituloCode { get; set; }
        public string? EspecialidadCode { get; set; }
        public string? TipoCentroFormacionCode { get; set; }
        public string? ModalidadFormativaCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
