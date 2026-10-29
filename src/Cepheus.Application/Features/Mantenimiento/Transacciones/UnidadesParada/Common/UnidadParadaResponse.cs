using Cepheus.Domain.Mantenimiento.Enum;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesParada.Common
{
    public class UnidadParadaResponse
    {
        public string SubCentroCostoCode { get; set; } = default!;
        public string SubCentroCostoName { get; set; } = default!;
        public DateTime Fecha { get; set; }
        public decimal Horas { get; set; }
        public EstadoEquipo Estado { get; set; }
        public string PlantaAfectadaCode { get; set; } = default!;
        public string PlantaAfectadaName { get; set; } = default!;
        public string? FallaCode { get; set; }
        public string? FallaName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
