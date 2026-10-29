using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Rrhh.Catalogos
{
    /// <summary>
    /// Tipo de extensión de contrato del trabajador. Catálogo del módulo RRHH.
    /// La extensión del contrato normalmente indica una característica adicional relacionada con la duración o extensión del vínculo contractual.
    /// Inicial, Renovación, Ampliación, Prórroga, Adenda
    /// Fuente: script RRHH provisto (rrhh.tipo_extension_contrato).
    /// Mapeo columnas script -> propiedades profesionales:
    ///   codigo  -> Code (PK natural, autogenerado correlativamente por la
    ///              aplicación — mismo patrón que Comunes.Banco / Logística.Familia)
    ///   nombre  -> Name
    ///   activo  -> IsActive
    /// </summary>
    public class TipoExtensionContrato : IAuditableEntity
    {
        /// <summary>
        /// Código del tipo de extensión de contrato (PK natural, hasta 20 caracteres, correlativo).
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