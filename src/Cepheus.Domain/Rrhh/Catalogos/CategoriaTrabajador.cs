using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Rrhh.Catalogos
{
    /// <summary>
    /// Categoría del trabajador. Catálogo del módulo RRHH.
    /// TRABAJADOR, PENSIONISTA, PRESTADOR DE SERVICIOS, PERSONAL DE TERCEROS, MODALIDAD FORMATIVA
    /// Fuente: script RRHH provisto (rrhh.categoria_trabajador).
    /// Mapeo columnas script -> propiedades profesionales:
    ///   codigo  -> Code (PK natural, autogenerado correlativamente por la
    ///              aplicación — mismo patrón que Comunes.Banco / Logística.Familia)
    ///   nombre  -> Name
    ///   activo  -> IsActive
    /// </summary>
    public class CategoriaTrabajador : IAuditableEntity
    {
        /// <summary>
        /// Código de la categoría de trabajador (PK natural, hasta 20 caracteres, correlativo).
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