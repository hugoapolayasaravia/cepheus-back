using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Administracion
{
    public class Permission : IAuditableEntity
    {
        public int Id { get; set; }
        public int ProgramaId { get; set; }
        public Programa Programa { get; set; } = default!;

        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;

        // Navegación
        public ICollection<PermissionRole> PermissionRoles { get; set; } = new List<PermissionRole>();
    }


}
