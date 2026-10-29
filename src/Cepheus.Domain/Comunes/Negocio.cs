namespace Cepheus.Domain.Comun
{
    /// <summary>
    /// Representa un negocio de la organización.
    ///
    /// El código es de 2 caracteres y constituye la clave primaria natural.
    /// Un negocio puede tener una o varias plantas.
    /// </summary>
    public class Negocio : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        // Auditoría
        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}