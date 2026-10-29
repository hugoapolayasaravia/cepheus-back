using Cepheus.Domain.Comun;
using Cepheus.Domain.Rrhh.Catalogos;

namespace Cepheus.Domain.Rrhh.Maestros
{
    /// <summary>
    /// Seguros médicos y SCTR del trabajador
    ///
    /// Legacy: rrhh.trabajador_seguro (SQL Server).
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
    public class TrabajadorSeguro : IAuditableEntity
    {
        public long Id { get; set; }

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public string? EpsCode { get; set; } // legacy: eps_id
        public Eps? Eps { get; set; }
        public string? SituacionEpsCode { get; set; } // legacy: situacion_eps_id
        public SituacionEps? SituacionEps { get; set; }
        public string? NumeroSeguro { get; set; } // legacy: numero_seguro
        public string? SctrTipoCode { get; set; } // legacy: sctr_tipo_id
        public TipoSctr? TipoSctr { get; set; }
        public string? SctrSaludCode { get; set; } // legacy: sctr_salud_id
        public SctrSalud? SctrSalud { get; set; }
        public string? SctrPensionCode { get; set; } // legacy: sctr_pension_id
        public SctrPension? SctrPension { get; set; }
        public bool EpsActivo { get; set; } // legacy: eps_activo (default 0)
        public bool SeguroMedico { get; set; } // legacy: seguro_medico (default 0)
        public bool EssaludVida { get; set; } // legacy: essalud_vida (default 0)

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
