using Cepheus.Domain.Comun;
using Cepheus.Domain.Rrhh.Catalogos;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Rrhh.Maestros
{
    /// <summary>
    /// Cuentas bancarias del trabajador. BancoCode/MonedaCode reutilizan Banco/Moneda ya existentes en Comunes
    ///
    /// Legacy: rrhh.trabajador_cuenta_bancaria (SQL Server).
    /// Relación con Trabajador: 1:N — puede haber varios registros por trabajador (histórico/lista).
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
    public class TrabajadorCuentaBancaria : IAuditableEntity
    {
        public long Id { get; set; }

        public string TrabajadorCode { get; set; } = default!;
        public Trabajador Trabajador { get; set; } = default!;

        public string? TipoCuentaCode { get; set; } // legacy: tipo_cuenta_id
        public TipoCuenta? TipoCuenta { get; set; }
        public string? BancoCode { get; set; } // FK -> Comunes.Banco.Code (reutiliza catálogo existente)
        public Banco? Banco { get; set; }
        public string? MonedaCode { get; set; } // FK -> Comunes.Moneda.Code (reutiliza catálogo existente)
        public Moneda? Moneda { get; set; }
        public string? NumeroCuenta { get; set; } // legacy: numero_cuenta
        public string TipoOperacion { get; set; } = default!; // legacy: tipo_operacion
        public bool Principal { get; set; } // legacy: principal (default 0)

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
