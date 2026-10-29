using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Catalogos;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Línea de detalle (producto cotizado) de una Cotizacion. Hijo del
    /// aggregate Cotizacion; no tiene ciclo de vida propio fuera de ella.
    ///
    /// Legacy: dbo.CotizacionDetalle (SQL Server). PK compuesta
    /// (Codigo_Neg, Ano_cot, Mes_cot, Codigo_cot, Item), donde Item es
    /// IDENTITY(1,1) NOT FOR REPLICATION: correlativo autogenerado por SQL
    /// Server, global (no reinicia por cotización). Se preserva ese
    /// comportamiento vía identity de BD.
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Neg, Ano_cot, Mes_cot, Codigo_cot -> FK compuesta -> Cotizacion
    ///   Item              -> Item (identity, parte de la PK)
    ///   Codigo_tpr+Codigo_prd -> ProductoTipoCode + ProductoCode (FK compuesta -> Producto)
    ///   Codigo_uni        -> UnitCode (FK -> UnidadMedidaVenta)
    ///   Cantidad_prd      -> Quantity (decimal(18,6))
    ///   Precio_prd        -> UnitPrice (decimal(18,4))
    ///   Total_prd         -> Total (columna calculada en el legacy,
    ///                        ROUND(Cantidad*Precio,2); acá se expone como
    ///                        propiedad de solo lectura calculada en memoria,
    ///                        no columna calculada en BD — mismo criterio que
    ///                        Ubigeo.FullAddress y Producto.FullCode).
    ///   Observacion_cot   -> Observations
    ///   Orden             -> Order
    ///   Cantidad_ent      -> DeliveredQuantity
    /// </summary>
    public class CotizacionDetalle : IAuditableEntity
    {
        public string NegocioCode { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public string Code { get; set; } = default!;
        public Cotizacion Cotizacion { get; set; } = default!;

        /// <summary>Correlativo autogenerado (IDENTITY en BD, global no reinicia por cotización).</summary>
        public int Item { get; set; }

        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public string UnitCode { get; set; } = default!;
        public UnidadMedidaVenta Unit { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        /// <summary>Calculado: Math.Round(Quantity * UnitPrice, 2). No persistido.</summary>
        public decimal Total => Math.Round(Quantity * UnitPrice, 2);

        public string Observations { get; set; } = default!;

        public int Order { get; set; }

        public decimal DeliveredQuantity { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
