using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Tipo de artículo (catálogo simple). Usado en el módulo de Logística.
    /// Tipo de administración o planificación
    ///
    /// Legacy: dbo.TTipoArticulo (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Tar       -> Code (PK natural, char(3), autogenerado
    ///                        correlativamente por la aplicación)
    ///   Descripcion_Tar  -> Name
    ///   IND_VIGENCIA,
    ///   FEC_INI, FEC_FIN -> descartados (decisión del negocio): se
    ///                        reemplazan por el IsActive estándar del resto
    ///                        de catálogos, sin vigencia por rango de fechas.
    /// </summary>
    public class TipoArticulo : IAuditableEntity
    {
        /// <summary>
        /// Código del tipo de artículo (PK natural, 3 caracteres, correlativo).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string Description { get; set; } = default!;

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