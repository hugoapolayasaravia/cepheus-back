using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Rrhh.Catalogos
{
    /// <summary>
    /// Cobertura de pensión del SCTR del trabajador. Catálogo del módulo RRHH.
    ///
    /// Fuente: script RRHH provisto (rrhh.sctr_pension).
    /// Mapeo columnas script -> propiedades profesionales:
    ///   codigo  -> Code (PK natural, autogenerado correlativamente por la
    ///              aplicación — mismo patrón que Comunes.Banco / Logística.Familia)
    ///   nombre  -> Name
    ///   activo  -> IsActive
    /// </summary>
    public class SctrPension : IAuditableEntity
    {
        /// <summary>
        /// Código de la cobertura de pensión SCTR (PK natural, hasta 20 caracteres, correlativo).
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