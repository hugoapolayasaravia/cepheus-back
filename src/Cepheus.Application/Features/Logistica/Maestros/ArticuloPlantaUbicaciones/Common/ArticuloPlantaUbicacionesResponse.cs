using System;

namespace Cepheus.Application.Features.Logistica.Maestros.ArticuloPlantaUbicaciones.Common
{
    public class ArticuloPlantaUbicacionesResponse
    {
        public string ArticuloCode { get; set; } = null!;
        public string ArticuloName { get; set; } = null!;
        public string PlantaCode { get; set; } = null!;
        public string PlantaName { get; set; } = null!;
        public string ArticuloUbicacionCode { get; set; } = null!;
        public string ArticuloUbicacionName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}