using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Administracion;

public class Role : IAuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Auditoría (IAuditableEntity)
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Concurrencia optimista
    public byte[] RowVersion { get; set; } = default!;

    // Navegación
    public ICollection<RoleUser> UserRoles { get; set; } = new List<RoleUser>();
    public ICollection<PermissionRole> PermissionRoles { get; set; } = new List<PermissionRole>();
}

