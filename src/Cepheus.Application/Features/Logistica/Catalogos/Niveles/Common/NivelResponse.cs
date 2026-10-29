namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.Common
{
    public class NivelResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
