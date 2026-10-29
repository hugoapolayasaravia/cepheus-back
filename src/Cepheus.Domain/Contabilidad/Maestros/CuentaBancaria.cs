using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;

namespace Cepheus.Domain.Contabilidad.Maestros
{
    /// <summary>
    /// Cuenta bancaria de la empresa, con su chequera y su cuenta contable. Maestro
    /// del módulo de Contabilidad, relacionado con Comunes.Banco, Comunes.Moneda y
    /// CuentaContable.
    ///
    /// Legacy: dbo.RCtaBancos (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales (se respetan los tipos
    /// de dato del legacy):
    ///   codigo_ban      -> BankCode (parte de la PK; FK -> Comunes.Banco.Code, char(3);
    ///                      el legacy tenía 2 caracteres, se migra al código de Banco)
    ///   numero_cta      -> AccountNumber (parte de la PK, char(20))
    ///   moneda_ban      -> CurrencyCode (FK -> Comunes.Moneda.Code, char(3); el legacy
    ///                      guardaba 1 carácter, se migra al código de Moneda)
    ///   CheInicial_cta  -> CheckStart (char(10), NOT NULL; primer cheque de la chequera)
    ///   CheFinal_cta    -> CheckEnd (char(10), NOT NULL; último cheque)
    ///   CheActual_cta   -> CheckCurrent (char(10), NOT NULL; cheque actual)
    ///   estado_cta      -> IsActive (el legacy usaba TEstados; solo se conoce el par
    ///                      '05' Activo / '06' Inactivo; si aparecen más valores en la
    ///                      data pasaría a enum)
    ///   control_cta     -> ControlFlag (char(1), NOT NULL; se conserva tal cual)
    ///   codigo_cta      -> AccountCode (FK -> CuentaContable.Code, char(8))
    /// </summary>
    public class CuentaBancaria : IAuditableEntity
    {
        public string BankCode { get; set; } = default!;
        public Banco Banco { get; set; } = default!;

        public string AccountNumber { get; set; } = default!;

        public string CurrencyCode { get; set; } = default!;
        public Moneda Currency { get; set; } = default!;

        public string CheckStart { get; set; } = string.Empty;
        public string CheckEnd { get; set; } = string.Empty;
        public string CheckCurrent { get; set; } = string.Empty;

        public string ControlFlag { get; set; } = default!;

        public string AccountCode { get; set; } = default!;
        public CuentaContable Account { get; set; } = default!;

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
