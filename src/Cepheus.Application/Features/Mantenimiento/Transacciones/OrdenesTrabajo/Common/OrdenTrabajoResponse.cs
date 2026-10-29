namespace Cepheus.Application.Features.Mantenimiento.Transacciones.OrdenesTrabajo.Common
{
    public class OrdenTrabajoResponse
    {
        public string PlantaCode { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string Description { get; set; } = default!;

        public DateTime FechaProceso { get; set; }
        public DateTime? FechaTermino { get; set; }

        public string ResponsableCode { get; set; } = default!;

        public string EspecialidadCode { get; set; } = default!;
        public string OportunidadCode { get; set; } = default!;
        public string EquipoCode { get; set; } = default!;
        public string PrioridadCode { get; set; } = default!;
        public string InspeccionCode { get; set; } = default!;
        public string TipoOrdenCode { get; set; } = default!;
        public string ActividadCode { get; set; } = default!;

        public string? CentroCostoCode { get; set; }
        public string? SubCentroCostoCode { get; set; }
        public string? SubCentroEjecutorCode { get; set; }
        public string? PlanMantenimientoPreventivoCode { get; set; }

        public decimal DowntimeHours { get; set; }
        public string Estado { get; set; } = default!;
        public decimal? Horometro { get; set; }
        public string? Observations { get; set; }
        public string Turno { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
