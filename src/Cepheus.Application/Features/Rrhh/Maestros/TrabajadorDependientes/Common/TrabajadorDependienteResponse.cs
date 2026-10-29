namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorDependientes.Common
{
    public class TrabajadorDependienteResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string? ParentescoCode { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Documento { get; set; }
        public bool Asegurado { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
