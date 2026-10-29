using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Maestros;

namespace Cepheus.Domain.Logistica.Maestros
{
    /// <summary>
    /// Cuenta contable por pagar de un proveedor en una moneda (cuentas 42 / 46). Es la cuenta que usa la provisión
    /// contable de compras (asiento tipo 02) para registrar la deuda con el proveedor según la moneda del documento.
    ///
    /// Reemplaza a las dos columnas del legacy en MProveedores:
    ///   enlace1_cta (soles) y enlace2_cta (dólares) -> una fila por proveedor y moneda, sin límite de monedas.
    ///
    /// PK: (ProveedorCode, MonedaCode).
    ///   ProveedorCode        FK Logistica.Proveedor
    ///   MonedaCode           FK Comunes.Moneda
    ///   AccountingAccountCode FK Contabilidad.CuentaContable; debe tener 8 caracteres y empezar con 42 o 46.
    /// </summary>
    public class ProveedorCuentaContable : IAuditableEntity
    {
        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public string AccountingAccountCode { get; set; } = default!;
        public CuentaContable CuentaContable { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
