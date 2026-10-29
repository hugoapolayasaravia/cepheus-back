using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Enum;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Atributo de la especificación técnica del concreto (resistencia, tipo de
    /// cemento, tamaño de piedra, slump, relación agua/cemento, edad, condición
    /// especial, proporción de mezcla). Catálogo único con discriminador
    /// AttributeType; lo referencia Producto (una FK por tipo de atributo).
    ///
    /// Legacy: dbo.MAE_TABLA_DET (SQL Server), tabla genérica a la que apuntaban
    /// las 8 columnas COD_* de MProductos. Se separa aquí solo el subconjunto de
    /// concreto. ESTRUCTURA SUPUESTA (pendiente de confirmar contra el legacy):
    ///   COD_TABLA -> Code (PK natural, hasta 4 caracteres, ingreso manual)
    ///   (descripción) -> Name
    ///   (tipo, deducido de la columna COD_* de MProductos que lo usa) -> AttributeType
    ///
    /// IsActive no existe en el legacy; se agrega por consistencia.
    /// </summary>
    public class AtributoConcreto : IAuditableEntity
    {
        /// <summary>Código del atributo (PK natural, hasta 4 caracteres, ej. "140", "V", "#57", "s34").</summary>
        public string Code { get; set; } = default!;

        public TipoAtributoConcreto AttributeType { get; set; }

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
