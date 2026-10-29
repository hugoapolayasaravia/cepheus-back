namespace Cepheus.Application.Features.Administracion.Permissions.Common
{
    public class PermissionResponse
    {
        public int Id { get; set; }
        public int ProgramaId { get; set; }
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }

}
