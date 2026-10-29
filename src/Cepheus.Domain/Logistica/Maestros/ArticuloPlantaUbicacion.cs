using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes; 
using System;

namespace Cepheus.Domain.Logistica.Maestros
{
    public class ArticuloPlantaUbicacion : IAuditableEntity
    {
        public string ArticuloCode { get; set; } = default!;
        public Articulo Articulo { get; set; } = default!;

        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        public string ArticuloUbicacionCode { get; set; } = default!;
        public ArticuloUbicacion ArticuloUbicacion { get; set; } = default!;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}