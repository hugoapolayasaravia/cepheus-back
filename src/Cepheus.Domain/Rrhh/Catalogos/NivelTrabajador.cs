using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Rrhh.Catalogos
{
    /// <summary>
    /// Nivel laboral del trabajador. Catálogo del módulo RRHH.
    ///
    /// Fuente: script RRHH provisto (rrhh.nivel). Nombrado "NivelTrabajador" en
    /// código (a pedido) para mayor claridad frente a otros posibles "Nivel"
    /// del sistema.
    ///
    /// Mapeo columnas script -> propiedades profesionales:
    ///   codigo  -> Code (PK natural, autogenerado correlativamente por la
    ///              aplicación — mismo patrón que Comunes.Banco / Logística.Familia)
    ///   nombre  -> Name
    ///   activo  -> IsActive
    /// </summary>
    public class NivelTrabajador : IAuditableEntity
    {
        /// <summary>
        /// Código del nivel (PK natural, hasta 20 caracteres, correlativo).
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