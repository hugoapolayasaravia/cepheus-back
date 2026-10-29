using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Segmento de ventas (catálogo simple). Agrupa los análisis de ventas
    /// (AnalisisVentas.SegmentoVentasCode) que luego se asignan a las obras de
    /// los clientes para el análisis comercial.
    ///
    /// Legacy: dbo.GES_SEGMENTO_VENTAS (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   COD_SEGMENTO  -> Code (PK natural, char(2))
    ///   DES_SEGMENTO  -> Name (varchar(50); NULL en el legacy, obligatorio acá:
    ///                    un segmento sin descripción no es identificable)
    ///   Codigo_est    -> reemplazado por IsActive estándar (FK legacy a
    ///                    dbo.TEstados; TEstados mezcla estados de distintas
    ///                    entidades y para este catálogo solo importa
    ///                    activo/inactivo, mismo criterio que Proveedor)
    /// </summary>
    public class SegmentoVentas : IAuditableEntity
    {
        /// <summary>
        /// Código del segmento (PK natural, 2 caracteres).
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
