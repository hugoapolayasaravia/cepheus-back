using Cepheus.Domain.Mantenimiento.Enum;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.PlanesPreventivo.Common
{
    public class PlanPreventivoResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string EquipoCode { get; set; } = default!;
        public string EquipoName { get; set; } = default!;
        public string EspecialidadCode { get; set; } = default!;
        public string EspecialidadName { get; set; } = default!;
        public string ResponsableCode { get; set; } = default!;
        public string ResponsableName { get; set; } = default!;
        public string SubCentroCostoCode { get; set; } = default!;
        public string SubCentroCostoName { get; set; } = default!;
        public string SubCentroEjecutorCode { get; set; } = default!;
        public string SubCentroEjecutorName { get; set; } = default!;
        public string ActividadCode { get; set; } = default!;
        /// <summary>Gama: verbo + objeto de la actividad (legacy Descripcion_Ver + ' ' + Descripcion_Sus).</summary>
        public string ActividadName { get; set; } = default!;
        public decimal Frecuencia { get; set; }
        public UnidadFrecuencia UnidadFrecuencia { get; set; }
        public decimal Factor { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }
        public decimal FrecuenciaGama { get; set; }
        public decimal TiempoRestante { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
