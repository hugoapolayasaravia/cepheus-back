using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Cuenta bancaria de un proveedor (tabla de detalle, N cuentas por
    /// proveedor).
    ///
    /// Legacy: dbo.MProveedorCuentas (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   IdCuenta      -> Id (PK real, IDENTITY)
    ///   Codigo_prv    -> ProveedorCode (FK -> Proveedor.Code)
    ///   BancoCodigo   -> BancoCode (FK -> Comunes.Banco.Code, catálogo nuevo)
    ///   TipoCuenta    -> AccountType (enum)
    ///   NumeroCuenta  -> AccountNumber
    ///   CCI           -> InterbankCode
    ///   Moneda        -> MonedaCode (FK -> Comunes.Moneda.Code)
    ///   EsPrincipal   -> IsPrimary (una sola cuenta principal por proveedor;
    ///                    exclusividad manejada en el handler)
    ///   Estado        -> reemplazado por IsActive estándar
    /// </summary>
    public class ProveedorCuenta : IAuditableEntity
    {
        public int Id { get; set; }

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string BancoCode { get; set; } = default!;
        public Banco Banco { get; set; } = default!;

        public AccountType AccountType { get; set; }

        public string AccountNumber { get; set; } = default!;
        public string? InterbankCode { get; set; }

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public bool IsPrimary { get; set; }

        public bool IsActive { get; set; } = true;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
