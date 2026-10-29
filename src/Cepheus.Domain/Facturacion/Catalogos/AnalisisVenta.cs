using Cepheus.Domain.Comun;

namespace Cepheus.Domain.Facturacion.Catalogos
{
    /// <summary>
    /// Análisis de ventas (catálogo). Clasificación comercial que se asigna a las
    /// obras de los clientes (Obra.AnalisisVentaCode) y que pertenece a un
    /// segmento de ventas.
    ///
    /// Se nombra en singular (AnalisisVenta) porque "análisis" es invariable en
    /// plural y la colección/namespace "AnalisisVentas" chocaría con el tipo.
    ///
    /// Legacy: dbo.GES_MAE_ANALISIS_VENTAS (SQL Server).
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   COD_ANALISIS        -> Code (PK natural, char(3), correlativo)
    ///   DES_ANALISIS        -> Name (varchar(40); NULL en el legacy, obligatorio
    ///                          acá)
    ///   DES_CORTA_ANALISIS  -> ShortName (varchar(8), opcional)
    ///   COD_SEGMENTO        -> SegmentoVentasCode (FK -> SegmentoVentas.Code,
    ///                          opcional como en el legacy)
    ///
    /// IsActive no existe en el legacy; se agrega por consistencia con el resto
    /// de catálogos del sistema.
    /// </summary>
    public class AnalisisVenta : IAuditableEntity
    {
        /// <summary>
        /// Código del análisis de ventas (PK natural, 3 caracteres).
        /// </summary>
        public string Code { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? ShortName { get; set; }

        public string? SegmentoVentasCode { get; set; }
        public SegmentoVentas? SegmentoVentas { get; set; }

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
