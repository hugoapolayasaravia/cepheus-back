namespace Cepheus.Application.Features.Logistica.Maestros.AprobadoresAsignados.Common
{
    public class AprobadorAsignadoResponse
    {
        public string NivelCode { get; set; } = default!;
        public string TipoTransaccionCode { get; set; } = default!;
        public string UnidadNegocioCode { get; set; } = default!;
        public string MonedaCode { get; set; } = default!;
        public string TrabajadorCode { get; set; } = default!;
        public string? SuplenteTrabajadorCode { get; set; }
        public string? SuperiorTrabajadorCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
