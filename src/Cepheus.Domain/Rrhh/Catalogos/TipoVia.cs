using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Rrhh.Catalogos
{
    /// <summary>
    /// Tipo de vía (Avenida, Jr., Calle, Pasaje). Catálogo del módulo RRHH,
    /// usado en el domicilio del trabajador.
    ///
    /// Fuente: script RRHH provisto (rrhh.tipo_via).
    /// Mapeo columnas script -> propiedades profesionales:
    ///   codigo      -> Code (PK natural, autogenerado correlativamente por la
    ///                  aplicación — mismo patrón que Comunes.Banco / Logística.Familia)
    ///   nombre      -> Name
    ///   abreviatura -> Abbreviation (opcional)
    ///   activo      -> IsActive
    /// </summary>
    public class TipoVia : IAuditableEntity
    {
        /// <summary>
        /// Código del tipo de vía (PK natural, hasta 10 caracteres, correlativo).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Abbreviation { get; set; }

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