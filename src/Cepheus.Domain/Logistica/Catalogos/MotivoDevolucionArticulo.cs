namespace Cepheus.Domain.Logistica.Catalogos
{
    public class MotivoDevolucionArticulo
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;
        public bool AffectsStock { get; set; }
        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
