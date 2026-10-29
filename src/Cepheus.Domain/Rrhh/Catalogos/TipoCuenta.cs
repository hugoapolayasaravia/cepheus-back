using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Rrhh.Catalogos
{
    /// <summary>
    /// Tipo de cuenta bancaria del trabajador (Ahorros, Corriente, etc.). Catálogo del módulo RRHH.
    ///
    /// Fuente: script RRHH provisto (rrhh.tipo_cuenta). Se crea como catálogo propio
    /// (con auditoría y RowVersion) en lugar de reusar Logistica.Enum.AccountType,
    /// que es un enum sin persistencia auditable.
    ///
    /// Mapeo columnas script -> propiedades profesionales:
    ///   codigo  -> Code (PK natural, autogenerado correlativamente por la
    ///              aplicación — mismo patrón que Comunes.Banco / Logística.Familia)
    ///   nombre  -> Name
    ///   activo  -> IsActive
    /// </summary>
    public class TipoCuenta : IAuditableEntity
    {
        /// <summary>
        /// Código del tipo de cuenta (PK natural, hasta 20 caracteres, correlativo).
        /// </summary>
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