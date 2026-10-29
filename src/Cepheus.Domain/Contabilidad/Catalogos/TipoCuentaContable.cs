using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Contabilidad.Catalogos
{
    /// <summary>
    /// Tipo de cuenta contable. Catálogo del módulo de Contabilidad, asignado a cada
    /// cuenta contable (MCuentas.Codigo_tcu).
    ///
    /// Se nombra con sufijo "Contable" porque Rrhh ya tiene su propia entidad
    /// TipoCuenta (tipo de cuenta bancaria del trabajador) — mismo criterio que
    /// Facturación con Transportista/Chofer/Vehiculo "Venta".
    ///
    /// Legacy: dbo.TTipCuentas (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_tcu      -> Code (PK natural, char(2), correlativo automático)
    ///   Descripcion_tcu -> Name (varchar(50) -> 100)
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia.
    /// </summary>
    public class TipoCuentaContable : IAuditableEntity
    {
        /// <summary>Código del tipo de cuenta contable (PK natural, 2 caracteres, correlativo).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

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
