namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorContactos.Common
{
    public class TrabajadorContactoResponse
    {
        public int Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Phone { get; set; }
        public string? ParentescoCode { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}