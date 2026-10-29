namespace Cepheus.Application.Features.Rrhh.Maestros.TrabajadorLaborals.Common
{
    public class TrabajadorLaboralResponse
    {
        public long Id { get; set; }
        public string TrabajadorCode { get; set; } = default!;
        public DateTime? FechaIngreso { get; set; }
        public DateTime? FechaCese { get; set; }
        public string? TipoTrabajadorCode { get; set; }
        public string? CategoriaTrabajadorCode { get; set; }
        public string? EstadoTrabajadorCode { get; set; }
        public string? AreaCode { get; set; }
        public string? OcupacionCode { get; set; }
        public string? SubOcupacionCode { get; set; }
        public string? CategoriaOcupacionalCode { get; set; }
        public string? SubCategoriaOcupacionalCode { get; set; }
        public string? OficinaCode { get; set; }
        public string? PlantaCode { get; set; }
        public string? CargoCode { get; set; }
        public string? NivelCode { get; set; }
        public string? RegimenLaboralCode { get; set; }
        public string? ProveedorCode { get; set; }
        public bool? Permanente { get; set; }
        public bool? Pensionista { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
