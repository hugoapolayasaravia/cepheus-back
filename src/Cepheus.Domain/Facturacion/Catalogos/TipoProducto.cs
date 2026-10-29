using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Clasificación o tipo al que pertenece un producto.
    /// </summary>
    public class TipoProducto : IAuditableEntity
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