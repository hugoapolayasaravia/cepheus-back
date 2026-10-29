using Cepheus.Domain.Comun;
using Cepheus.Domain.Rrhh.Catalogos;

namespace Cepheus.Domain.Rrhh.Maestros
{
    /// <summary>
    /// Jornada y control horario del trabajador
    ///
    /// Legacy: rrhh.trabajador_jornada (SQL Server).
    /// Relación con Trabajador: 1:1 — un único registro por trabajador (UNIQUE trabajador_id en el legacy).
    ///
    /// NOTA DE INTEGRACIÓN — pendiente de ajuste por el equipo:
    ///   - Se asume que la entidad Trabajador vive en
    ///     Cepheus.Domain.Rrhh.Trabajador con Id (long) como PK. Si tu
    ///     implementación usa otro namespace/tipo de PK, ajusta
    ///     TrabajadorId y la navegación.
    ///   - Se asume que los 45 catálogos de rrhh (Sexo, EstadoCivil,
    ///     Nacionalidad, Area, Cargo, etc.) viven en
    ///     Cepheus.Domain.Rrhh.Catalogos, con Id (long) como PK — igual
    ///     patrón que uses tú al crearlos. Si el namespace o el nombre de
    ///     alguna clase difiere, solo hay que corregir el using y el tipo
    ///     de la navegación, la forma de la entidad no cambia.
    ///   - moneda_id/banco_id del legacy se adaptaron para reutilizar los
    ///     catálogos Moneda/Banco YA EXISTENTES en Comunes (por su Code
    ///     natural, no por un Id bigint que esas tablas no tienen).
    ///
    /// IsActive/CreatedAt/CreatedBy/UpdatedAt/UpdatedBy/RowVersion no
    /// existen en el script legacy; se agregan por consistencia con el
    /// resto de entidades del sistema.
    /// </summary>
    public class TrabajadorJornada : IAuditableEntity
    {
        public long Id { get; set; }

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public string? HorarioCode { get; set; } // legacy: horario_id
        public Horario? Horario { get; set; }
        public bool HorasExtras { get; set; } // legacy: horas_extras (default 0)
        public bool HorasExt40 { get; set; } // legacy: horas_ext40 (default 0)
        public bool HorasExtCon { get; set; } // legacy: horas_ext_con (default 0)
        public decimal HorasExtCon125 { get; set; } // legacy: horas_ext_con_125 (default 0)
        public decimal HorasExtCon135 { get; set; } // legacy: horas_ext_con_135 (default 0)
        public bool ControlHorario { get; set; } // legacy: control_horario (default 0)
        public bool HorarioOrdinario { get; set; } // legacy: horario_ordinario (default 0)
        public bool HorarioNocturno { get; set; } // legacy: horario_nocturno (default 0)
        public bool JornadaMaxima { get; set; } // legacy: jornada_maxima (default 0)
        public bool RegimenAlternativo { get; set; } // legacy: regimen_alternativo (default 0)

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
