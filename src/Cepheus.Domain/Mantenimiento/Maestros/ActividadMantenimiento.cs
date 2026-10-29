using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Maestros
{
    /// <summary>
    /// Regla de cobertura entre actividades de mantenimiento preventivo: al
    /// ejecutarse una orden de trabajo de ActividadCode, se reinicia el ciclo
    /// de las actividades que cubre (ActividadCubiertaCode) del mismo equipo.
    /// Prioridad define el orden en que el calendario de equipos procesa las
    /// actividades de un equipo (menor primero: las cubiertas antes que las
    /// que las cubren).
    ///
    /// Legacy: dbo.TActividadMantenimiento (SQL Server), PK compuesta
    /// (Codigo_Act, Codigo_ActCon).
    ///   Codigo_Act    -> ActividadCode (FK -> Actividad)
    ///   Codigo_ActCon -> ActividadCubiertaCode (FK -> Actividad). Nombre
    ///                    deducido del comportamiento de
    ///                    Man_sp_Listado_Calendario_Equipos; una fila con
    ///                    ActividadCode = ActividadCubiertaCode es válida y
    ///                    solo sirve para asignar la prioridad a una actividad
    ///                    que no cubre otras.
    ///   Prioridad     -> Prioridad (int, default 0)
    /// </summary>
    public class ActividadMantenimiento : IAuditableEntity
    {
        public string ActividadCode { get; set; } = default!;
        public Actividad Actividad { get; set; } = default!;

        public string ActividadCubiertaCode { get; set; } = default!;
        public Actividad ActividadCubierta { get; set; } = default!;

        public int Prioridad { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
