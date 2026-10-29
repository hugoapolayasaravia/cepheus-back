using Cepheus.Domain.Comun;
using Cepheus.Domain.Rrhh.Catalogos;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Rrhh.Maestros
{
    /// <summary>
    /// Remuneración del trabajador. MonedaCode reutiliza el catálogo Moneda ya existente en Comunes (por Code, no por Id, ya que Moneda no usa PK bigint)
    ///
    /// Legacy: rrhh.trabajador_remuneracion (SQL Server).
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
    public class TrabajadorRemuneracion : IAuditableEntity
    {
        public long Id { get; set; }

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public decimal SueldoBasico { get; set; } // legacy: sueldo_basico (default 0)
        public string? MonedaCode { get; set; } // FK -> Comunes.Moneda.Code (reutiliza catálogo existente)
        public Moneda? Moneda { get; set; }
        public string? ModoPagoCode { get; set; } // legacy: modo_pago_id
        public ModoPago? ModoPago { get; set; }

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
