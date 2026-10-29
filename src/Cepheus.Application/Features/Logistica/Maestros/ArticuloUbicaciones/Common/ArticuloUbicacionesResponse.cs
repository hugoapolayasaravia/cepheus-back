namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloUbicaciones.Common
{
    public class ArticuloUbicacionesResponse
    {
        public string Code { get; set; } = default!;
        public string Name { get; set; } = default!;

        public string PlantaCode { get; set; } = default!;
        public string? PlantaName { get; set; } // Útil para mostrar en UI/Grillas

        public string? Zone { get; set; }
        public string? Rack { get; set; }
        public string? Shelf { get; set; }
        public string? Locker { get; set; }

        public bool IsActive { get; set; }

        // Datos de auditoría
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
