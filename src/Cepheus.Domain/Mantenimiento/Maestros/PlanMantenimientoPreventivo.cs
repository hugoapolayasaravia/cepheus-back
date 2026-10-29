using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Mantenimiento.Catalogos;
using Cepheus.Domain.Mantenimiento.Enum;
using Cepheus.Domain.Rrhh.Maestros;

namespace Cepheus.Domain.Mantenimiento.Maestros
{
    /// <summary>
    /// Plan (plantilla) de Mantenimiento Preventivo de un equipo: qué actividad
    /// ejecutar, con qué frecuencia, quién es el responsable, y sus recursos
    /// (mano de obra y materiales, ver PlanMantenimientoPreventivoManoObra y
    /// PlanMantenimientoPreventivoMaterial). A partir de él se generan las
    /// Órdenes de Trabajo preventivas.
    ///
    /// Legacy: dbo.MManPreventivo (SQL Server), PK compuesta
    /// (Codigo_Pla, Codigo_Mpv). Se mantiene la PK compuesta
    /// (PlantaCode, Code), igual que OrdenTrabajo.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Mpv        -> Code ('MP' + correlativo de 4 dígitos por planta,
    ///                        ej. MP0001; lo genera el backend)
    ///   Descripcion_Mpv   -> Description
    ///   Fecha_Pro         -> eliminado (el script legado lo marca "reemplazar
    ///                        por variables de auditoría" -> CreatedAt)
    ///   Codigo_Ori        -> EquipoCode (FK -> Equipo, la MOrigenes legacy)
    ///   Codigo_Esp        -> EspecialidadCode (FK -> Especialidad)
    ///   Codigo_Res        -> ResponsableCode (FK -> Rrhh.Trabajador)
    ///   Codigo_Scc        -> SubCentroCostoCode (FK -> SubCentroCosto)
    ///   Codigo_Sce        -> SubCentroEjecutorCode (FK -> SubCentroEjecutor)
    ///   Codigo_act        -> ActividadCode (FK -> Actividad)
    ///   Frecuencia        -> Frecuencia (decimal(12,2))
    ///   Unidad            -> UnidadFrecuencia (enum)
    ///   Factor            -> Factor (decimal(12,2))
    ///   Fecha_Ini/Ter     -> FechaInicio / FechaTermino (nullable)
    ///   FrecuenciaGama    -> FrecuenciaGama (decimal(18,0)): frecuencia
    ///                        completa de la gama (ciclo), base del calendario
    ///   TiempoResta_Mpv   -> TiempoRestante (decimal(18,0)): tiempo que falta
    ///                        para el próximo preventivo, base del calendario
    /// </summary>
    public class PlanMantenimientoPreventivo : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public Planta Planta { get; set; } = default!;

        /// <summary>'MP' + 4 dígitos por planta (Codigo_Mpv, char(6) en el legacy).</summary>
        public string Code { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string EquipoCode { get; set; } = default!;
        public Equipo Equipo { get; set; } = default!;

        public string EspecialidadCode { get; set; } = default!;
        public Especialidad Especialidad { get; set; } = default!;

        public string ResponsableCode { get; set; } = default!;
        public Trabajador Responsable { get; set; } = default!;

        public string SubCentroCostoCode { get; set; } = default!;
        public SubCentroCosto SubCentroCosto { get; set; } = default!;

        public string SubCentroEjecutorCode { get; set; } = default!;
        public SubCentroEjecutor SubCentroEjecutor { get; set; } = default!;

        public string ActividadCode { get; set; } = default!;
        public Actividad Actividad { get; set; } = default!;

        public decimal Frecuencia { get; set; }
        public UnidadFrecuencia UnidadFrecuencia { get; set; }
        public decimal Factor { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaTermino { get; set; }

        public decimal FrecuenciaGama { get; set; }
        public decimal TiempoRestante { get; set; }

        public ICollection<PlanMantenimientoPreventivoManoObra> ManoObra { get; set; } = new List<PlanMantenimientoPreventivoManoObra>();
        public ICollection<PlanMantenimientoPreventivoMaterial> Materiales { get; set; } = new List<PlanMantenimientoPreventivoMaterial>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
