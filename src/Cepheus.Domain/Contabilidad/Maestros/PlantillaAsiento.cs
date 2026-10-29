using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Contabilidad.Catalogos;
using Cepheus.Domain.Contabilidad.Enum;

namespace Cepheus.Domain.Contabilidad.Maestros
{
    /// <summary>
    /// Plantilla de asiento: define, por proceso contable, cómo se arma el asiento (qué líneas, en qué
    /// naturaleza, con qué cuenta e importe). Reemplaza a las cuentas y reglas que el legacy tenía escritas en
    /// el programa (PowerBuilder): IGV 40110101, IGV exterior 40110102, renta 40170103, fonavi 40170106,
    /// compras 60610101, transferencias 28110102 / 26110102, tipo de asiento "02".
    ///
    /// Una plantilla ACTIVA por proceso. Las cuentas son FK a CuentaContable (no hay cuentas en el código).
    /// </summary>
    public class PlantillaAsiento : IAuditableEntity
    {
        /// <summary>Código correlativo (PK natural, 3 caracteres).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public ProcesoAsiento Process { get; set; }

        /// <summary>Tipo de asiento con el que se genera (por ejemplo "02").</summary>
        public string TipoAsientoCode { get; set; } = default!;
        public TipoAsiento TipoAsiento { get; set; } = default!;

        /// <summary>
        /// Cuenta de compras con la que se registran las líneas del documento de compra generado desde una
        /// nota de ingreso (legacy: 60610101 en duro). Obligatoria en ProvisionCompraNotaIngreso; no aplica en
        /// transferencias.
        /// </summary>
        public string? PurchaseAccountCode { get; set; }
        public CuentaContable? PurchaseAccount { get; set; }

        /// <summary>
        /// Comprobante de pago con el que se registran las líneas del asiento cuando no proviene de un documento de
        /// compra (consumo de vales: "00"). Obligatorio en ConsumoVales.
        /// </summary>
        public string? ComprobantePagoCode { get; set; }
        public ComprobantePago? ComprobantePago { get; set; }

        /// <summary>
        /// Texto fijo de la descripción del asiento y de sus líneas (por ejemplo "ASIENTO DE CONSUMOS").
        /// Obligatorio en ConsumoVales.
        /// </summary>
        public string? DescriptionText { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<PlantillaAsientoLinea> Lines { get; set; } = new List<PlantillaAsientoLinea>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
