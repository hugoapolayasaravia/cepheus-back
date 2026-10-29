using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Administracion
{
    public class Submodulo : IAuditableEntity
    {
        public int Id { get; set; }
        public int ModuloId { get; set; }
        public Modulo Modulo { get; set; } = default!;

        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string? Icon { get; set; }
        public string? Tooltip { get; set; }
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
        public ICollection<Programa> Programas { get; set; } = new List<Programa>();
    }



}
