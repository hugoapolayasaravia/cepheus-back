using Cepheus.Domain.Administracion;
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Mantenimiento.Catalogos;
using Cepheus.Domain.Mantenimiento.Enum;
using Cepheus.Domain.Mantenimiento.Maestros;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Mantenimiento.Transacciones
{
    /// <summary>
    /// Orden de Trabajo: cabecera de una tarea de mantenimiento. Transacción
    /// principal del módulo Mantenimiento.
    ///
    /// Legacy: dbo.MOrdenTrabajos (SQL Server), PK compuesta
    /// (Codigo_Pla, Codigo_Otr). Se mantiene la PK compuesta
    /// (PlantaCode, Code) — mismo criterio que ArticuloProveedor en
    /// Logística.
    ///
    /// Cambios frente al legacy (ver análisis previo aprobado):
    ///   - Codigo_Cos + Codigo_Co1 (duplicados) -> una sola CentroCostoCode
    ///   - Codigo_Scc + Codigo_Sc1 (duplicados) -> una sola SubCentroCostoCode
    ///   - Codigo_Est (varchar libre con texto) -> Estado (enum EstadoOrdenTrabajo)
    ///   - Turno_Otr (char(1)) -> Turno (enum)
    ///   - Codigo_Usu (varchar(20) libre) -> UserId (FK -> Administracion.User)
    ///   - Cod_Planta (duplicado de Codigo_Pla) -> eliminado
    ///   - Codigo_spla (sub-planta) -> eliminado, confirmado por el usuario
    ///     (no existe ese catálogo)
    ///   - Horometro (char(5)) -> decimal? (era una medición numérica)
    ///   - Tie_fal -> DowntimeHours (decimal)
    ///
    /// FKs pendientes de habilitar cuando existan sus tablas (se guardan como
    /// código simple, sin relación de navegación EF, hasta entonces):
    ///   - Codigo_Res -> ResponsableCode (Trabajador, módulo en construcción)
    ///   - Codigo_Mpv -> PlanMantenimientoPreventivoCode (módulo futuro)
    /// </summary>
    public class OrdenTrabajo : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        /// <summary>Correlativo por planta (Codigo_Otr, char(6) en el legacy).</summary>
        public string Code { get; set; } = default!;

        public string Description { get; set; } = default!;

        public DateTime FechaProceso { get; set; }
        public DateTime? FechaTermino { get; set; }

        // Pendiente de FK real: Trabajador aún no existe como tabla
        public string ResponsableCode { get; set; } = default!;
        public Trabajador Responsable { get; set; } = default!;

        public string EspecialidadCode { get; set; } = default!;
        public Especialidad Especialidad { get; set; } = default!;

        public string OportunidadCode { get; set; } = default!;
        public Oportunidad Oportunidad { get; set; } = default!;

        public string EquipoCode { get; set; } = default!;
        public Equipo Equipo { get; set; } = default!;

        public string PrioridadCode { get; set; } = default!;
        public Prioridad Prioridad { get; set; } = default!;

        public string InspeccionCode { get; set; } = default!;
        public Inspeccion Inspeccion { get; set; } = default!;

        public string TipoOrdenCode { get; set; } = default!;
        public TipoOrden TipoOrden { get; set; } = default!;

        public string ActividadCode { get; set; } = default!;
        public Actividad Actividad { get; set; } = default!;

        public string? SubCentroCostoCode { get; set; }
        public SubCentroCosto? SubCentroCosto { get; set; }

        public string? SubCentroEjecutorCode { get; set; }
        public SubCentroEjecutor? SubCentroEjecutor { get; set; }

        // Pendiente de FK real: módulo de Mantenimiento Preventivo (planes) aún no existe
        public string? PlanMantenimientoPreventivoCode { get; set; }

        public decimal DowntimeHours { get; set; }

        public EstadoOrdenTrabajo Estado { get; set; } = EstadoOrdenTrabajo.Pendiente;

        public decimal? Horometro { get; set; }

        public string? Observaciones { get; set; }

        public Turno Turno { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
