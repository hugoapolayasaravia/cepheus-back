using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Mantenimiento.Maestros
{
    /// <summary>
    /// Actividad de mantenimiento: combinación válida de un
    /// VerboActividad + un ObjetoActividad (ej. "LIMPIAR" + "ENFRIADOR
    /// ACEITE MOTOR"). Maestro del módulo Mantenimiento.
    ///
    /// Legacy: no existía como tabla propia — Codigo_Act era un campo de 6
    /// caracteres directo en dbo.MOrdenTrabajos, compuesto manualmente por
    /// el usuario como Codigo_Ver (3) + Codigo_Sus (3), sin catálogo de
    /// combinaciones válidas detrás (ej. "LIM193" en el dato de muestra).
    ///
    /// Se crea Actividad como maestro nuevo para que el sistema valide y
    /// reutilice combinaciones en vez de aceptar cualquier concatenación:
    ///   Code -> calculado por el backend = VerboActividadCode +
    ///           ObjetoActividadCode (6 caracteres), NO lo digita el
    ///           usuario — igual criterio que SubFamilia en Logística.
    ///
    /// A diferencia del resto de maestros del módulo, Actividad NO tiene
    /// operación de edición: cambiar el Verbo o el Objeto cambia el propio
    /// Code (es, en la práctica, otra Actividad). Si una combinación ya no
    /// se usa, se desactiva con IsActive en vez de "editarla".
    /// </summary>
    public class Actividad : IAuditableEntity
    {
        public string Code { get; set; } = default!;

        public string VerboActividadCode { get; set; } = default!;
        public VerboActividad VerboActividad { get; set; } = default!;

        public string ObjetoActividadCode { get; set; } = default!;
        public ObjetoActividad ObjetoActividad { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
