using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Logistica.Maestros
{
    public class ArticuloUbicacion : IAuditableEntity
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string? Zone { get; set; }     // Ej: "Zona Repuestos", "Taller Central"
        public string? Rack { get; set; }     // Ej: "R-01"
        public string? Shelf { get; set; }    // Ej: "E-03"
        public string? Locker { get; set; }   // Ej: "C-12" (Casillero/Nivel)

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
