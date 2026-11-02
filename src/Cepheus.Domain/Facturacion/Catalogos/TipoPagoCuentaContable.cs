using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Maestros;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Cuenta contable de un tipo de pago por moneda. Reemplaza a TTipPago.Cuenta_sol / Cuenta_dol del legacy
    /// ("NO VA"): por ejemplo Letras (03) y Letras Varias (11) cargan 12310101 en soles y 12310102 en dólares.
    /// PK: (TipoPagoCode, MonedaCode).
    /// </summary>
    public class TipoPagoCuentaContable : IAuditableEntity
    {
        public string TipoPagoCode { get; set; } = default!;
        public TipoPago TipoPago { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public string AccountCode { get; set; } = default!;
        public CuentaContable Account { get; set; } = default!;

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
