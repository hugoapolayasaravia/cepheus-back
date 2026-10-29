using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Tipo de bien o servicio sujeto a detracción, con su tasa. Catálogo del
    /// módulo de Facturación y Ventas, referenciado por Producto.
    ///
    /// Legacy: dbo.COM_BIENES_TIPO (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   COD_TIPO_BIENES -> Code (PK natural, char(3); el código lo define la SUNAT,
    ///                      por eso se ingresa manualmente, no es correlativo)
    ///   DES_TIPO_BIENES -> Name
    ///   POR_TASA_BIEN   -> DetractionRate (decimal(5,2), porcentaje)
    ///
    /// IsActive no existe en la tabla legacy; se agrega por consistencia.
    /// </summary>
    public class TipoBien : IAuditableEntity
    {
        /// <summary>Código del tipo de bien (PK natural, 3 caracteres; lo define la SUNAT).</summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        /// <summary>Tasa de detracción en porcentaje (0 a 100).</summary>
        public decimal DetractionRate { get; set; }

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
