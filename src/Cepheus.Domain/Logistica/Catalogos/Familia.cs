using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Familia de artículos, catálogo raíz de la clasificación de inventario
    /// (Familia -> SubFamilia). Usado en el módulo de Logística.
    ///
    /// Legacy: dbo.TMFamilias (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Fam       -> Code (PK natural, char(2) — NO se genera un Id
    ///                        surrogate; se respeta la clave del legacy porque
    ///                        TMSubFamilias.Codigo_Fam depende de este valor
    ///                        como FK)
    ///   Descripcion_Fam  -> Name
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos del sistema (mismo caso que Ubigeo/TipoDocumento).
    /// </summary>
    public class Familia : IAuditableEntity
    {
        /// <summary>
        /// Código de la familia (PK natural, 2 caracteres). Inmutable luego de
        /// creado: es referenciado por TMSubFamilias.Codigo_Fam en el legacy.
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

        // Navegación
        public ICollection<SubFamilia> SubFamilias { get; set; } = new List<SubFamilia>();
    }
}