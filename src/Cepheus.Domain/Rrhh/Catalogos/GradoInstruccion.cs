using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Rrhh.Catalogos
{
    /// <summary>
    /// Grado académico del trabajador. Catálogo del módulo RRHH.
    /// Grado académico obtenido: Bachiller, Titulado, Maestro, Doctor
    /// Fuente: script RRHH provisto (rrhh.grado_instruccion).
    ///   nombre  -> Name
    ///   activo  -> IsActive
    /// </summary>
    public class GradoInstruccion : IAuditableEntity
    {
        /// <summary>
        /// Código del grado de instrucción (PK natural, hasta 20 caracteres, correlativo).
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