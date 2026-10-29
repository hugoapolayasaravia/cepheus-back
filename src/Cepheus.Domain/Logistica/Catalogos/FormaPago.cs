using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Logistica.Catalogos
{
    /// <summary>
    /// Forma de pago para compras (catálogo simple, con plazo y condición de
    /// crédito). Usado en el módulo de Logística.
    ///
    /// Legacy: dbo.TForPagCompras (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_pgc    -> Code (PK natural, char(2), autogenerado
    ///                     correlativamente por la aplicación)
    ///   Descripcion_pgc -> Name
    ///   dias_pgc      -> Days (plazo en días)
    ///   credito_pgv   -> IsCredit (char(1) 'S'/'N' en el legacy, default
    ///                     'N' -> se mapea a bool, default false)
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia con
    /// el resto de catálogos del sistema.
    /// </summary>
    public class FormaPago : IAuditableEntity
    {
        /// <summary>
        /// Código de la forma de pago (PK natural, 2 caracteres, correlativo).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        /// <summary>
        /// Plazo de pago, en días.
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Indica si la forma de pago es al crédito. Default: false (equivale
        /// al 'N' por defecto del legacy).
        /// </summary>
        public bool IsCredit { get; set; }

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