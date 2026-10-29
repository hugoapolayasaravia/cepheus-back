namespace Cepheus.Application.Features.Mantenimiento.Maestros.Equipos.Common
{
    public class EquipoResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int Nivel { get; set; }
        public string? SubCentroCostoCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
