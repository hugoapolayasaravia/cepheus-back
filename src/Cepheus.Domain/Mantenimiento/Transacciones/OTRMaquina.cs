using Cepheus.Domain.Comun;
using Cepheus.Domain.Mantenimiento.Catalogos;

namespace Cepheus.Domain.Mantenimiento.Transacciones
{
    /// <summary>
    /// Registro de uso de una Máquina en una Orden de Trabajo. Detalle hijo
    /// de OrdenTrabajo.
    ///
    /// Legacy: dbo.TOTRMaquinas (SQL Server), PK compuesta
    /// (Codigo_Pla, Codigo_Otr, Codigo_Maq) — a diferencia de OTResponsables,
    /// la fecha NO forma parte de la clave: solo puede existir un registro
    /// de uso por máquina y por OT (si la máquina se usa varias veces en la
    /// misma OT, se actualiza Cantidad/Horas del registro existente, no se
    /// crean filas nuevas).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Pla + Codigo_Otr -> PlantaCode + OrdenTrabajoCode
    ///                              (FK compuesta -> OrdenTrabajo)
    ///   Codigo_Maq                -> MaquinaCode (FK -> Maquina, catálogo
    ///                                ya creado en Mantenimiento.Catalogos)
    ///   Fecha_Pro                 -> FechaProceso (default hoy)
    ///   Cantidad                  -> Cantidad (decimal(12,5), default 0)
    ///   Hora                      -> Horas (legacy char(5), formato de hora
    ///                                tipo "08:00"; se modela como decimal
    ///                                — horas trabajadas de la máquina —
    ///                                igual criterio que Horometro en
    ///                                OrdenTrabajo)
    /// </summary>
    public class OTRMaquina : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string OrdenTrabajoCode { get; set; } = default!;
        public OrdenTrabajo OrdenTrabajo { get; set; } = default!;

        public string MaquinaCode { get; set; } = default!;
        public Maquina Maquina { get; set; } = default!;

        public DateTime FechaProceso { get; set; }

        public decimal Cantidad { get; set; }
        public decimal Horas { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
