using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Rrhh.Catalogos
{
    /// <summary>
    /// Tipo de zona (Urbanización, AA.HH., Caserío). Catálogo del módulo RRHH,
    /// usado en el domicilio del trabajador.
    ///
    /// Fuente: script RRHH provisto (rrhh.tipo_zona).
    /// Mapeo columnas script -> propiedades profesionales:
    ///   codigo  -> Code (PK natural, autogenerado correlativamente por la
    ///              aplicación — mismo patrón que Comunes.Banco / Logística.Familia)
    ///   nombre  -> Name
    ///   activo  -> IsActive
    /// </summary>
    public class TipoZona : IAuditableEntity
    {
        /// <summary>
        /// Código del tipo de zona (PK natural, hasta 10 caracteres, correlativo).
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