using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Catalogos;

namespace Cepheus.Domain.Contabilidad.Maestros
{
    /// <summary>
    /// Cuenta contable (plan de cuentas). Maestro del módulo de Contabilidad,
    /// relacionado con TipoCuentaContable, TipoAnexo y Comunes.Moneda.
    ///
    /// Legacy: dbo.MCuentas (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales (se respetan los tipos
    /// de dato del legacy; las letras/flags NO se convierten a bool ni enum):
    ///   Codigo_cta       -> Code (PK natural, char(8); lo define el plan de cuentas,
    ///                       por eso se ingresa manualmente, no es correlativo)
    ///   Descripcion_cta  -> Name (varchar(70))
    ///   Codigo_tcu       -> TipoCuentaCode (FK -> TipoCuentaContable.Code, char(2))
    ///   Codigo_tip       -> TipoAnexoCode (FK -> TipoAnexo.Code, char(3))
    ///   Enlace1_cta      -> LinkAccount1Code (char(8), NOT NULL; cuenta de enlace 1
    ///                       para destinos; SIN relación, tal cual el legacy)
    ///   Enlace2_cta      -> LinkAccount2Code (char(8), NOT NULL; ídem, sin relación)
    ///   Moneda_cta       -> CurrencyCode (FK -> Comunes.Moneda.Code, char(3); el legacy
    ///                       guardaba 1 carácter, se migra al código de Comunes.Moneda)
    ///   Acm_cta          -> AcmFlag (char(1), NOT NULL; flag de ACm)
    ///   piloto           -> PilotFlag (char(1), NOT NULL; 'S' | 'N')
    ///   codigo_gyp       -> GainsLossesFlag (char(1), NOT NULL; flag de pertenencia a
    ///                       Ganancias y Pérdidas)
    ///   Cuenta_cie       -> ClosingAccountCode (char(8), NOT NULL; cuenta para el cierre
    ///                       anual; SIN relación, tal cual el legacy)
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia.
    /// </summary>
    public class CuentaContable : IAuditableEntity
    {
        /// <summary>Código de la cuenta (PK natural, hasta 8 caracteres, ingresado).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string TipoCuentaCode { get; set; } = default!;
        public TipoCuentaContable TipoCuenta { get; set; } = default!;

        public string? TipoAnexoCode { get; set; } = default!;
        public TipoAnexo? TipoAnexo { get; set; } = default!;

        public string LinkAccount1Code { get; set; } = string.Empty;
        public string LinkAccount2Code { get; set; } = string.Empty;

        public string CurrencyCode { get; set; } = default!;
        public Moneda Currency { get; set; } = default!;

        public string? AcmFlag { get; set; } = default!;
        public string? PilotFlag { get; set; } = default!;
        public string? GainsLossesFlag { get; set; } = default!;

        public string ClosingAccountCode { get; set; } = string.Empty;

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
