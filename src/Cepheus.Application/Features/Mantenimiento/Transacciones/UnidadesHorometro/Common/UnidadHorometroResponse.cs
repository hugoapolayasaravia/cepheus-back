using Cepheus.Domain.Mantenimiento.Enum;

namespace Cepheus.Application.Features.Mantenimiento.Transacciones.UnidadesHorometro.Common
{
    public class UnidadHorometroResponse
    {
        public string SubCentroCostoCode { get; set; } = default!;
        public string SubCentroCostoName { get; set; } = default!;
        public DateTime Fecha { get; set; }
        public decimal Horometro { get; set; }
        public EstadoEquipo Estado { get; set; }
        public string PlantaAfectadaCode { get; set; } = default!;
        public string PlantaAfectadaName { get; set; } = default!;
        public string? EquipoCode { get; set; }
        public string? EquipoName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
