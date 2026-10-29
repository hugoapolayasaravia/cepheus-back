namespace Cepheus.Application.Features.Logistica.Catalogos.UsuariosPlanta.Common
{
    public class UsuarioPlantaResponse
    {
        public int UserId { get; set; }
        public string UserEmail { get; set; } = default!;
        public string UserFullName { get; set; } = default!;
        public string PlantaCode { get; set; } = default!;
        public string PlantaName { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
