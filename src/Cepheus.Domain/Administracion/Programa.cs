using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Administracion
{
    public class Programa : IAuditableEntity
    {
        public int Id { get; set; }
        public int SubmoduloId { get; set; }
        public Submodulo Submodulo { get; set; } = default!;

        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Icon { get; set; }
        public string? Tooltip { get; set; }
        public string? Route { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;

        // Navegación
        public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    }



}
