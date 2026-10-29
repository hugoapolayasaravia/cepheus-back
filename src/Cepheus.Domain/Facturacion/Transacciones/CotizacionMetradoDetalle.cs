using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Enum;
using Cepheus.Domain.Facturacion.Maestros;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Línea de detalle del metrado (cálculo de material por paño/tramo)
    /// dentro de un nivel de una Cotizacion. Hijo de CotizacionMetradoResumen,
    /// nieto del aggregate Cotizacion.
    ///
    /// Legacy: dbo.CotizacionesMetradoDet (SQL Server). Tabla más ancha del
    /// módulo (37 columnas propias). PK compuesta (Codigo_Neg, Ano_cot,
    /// Mes_cot, Codigo_Cot, Numero_Niv, Orden_Cot, Codigo_Tpr, Codigo_Prd).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales. Varias columnas
    /// son parámetros de cálculo estructural específicos del negocio
    /// (bovedillas/losas aligeradas) sin comentario en el DDL original:
    /// se traducen literalmente conservando el nombre de dominio y se marca
    /// PENDIENTE CONFIRMAR donde el propósito exacto no es inequívoco a
    /// partir del nombre de columna. Se recomienda revisar esta lista
    /// contigo antes de congelar el modelo, dado el volumen de columnas.
    ///
    ///   Codigo_Neg..Numero_Niv -> FK compuesta -> CotizacionMetradoResumen
    ///   Orden_Cot        -> Order (char(3), parte de la PK)
    ///   Codigo_Tpr+Codigo_Prd -> ProductoTipoCode + ProductoCode (FK compuesta
    ///                     -> Producto, parte de la PK)
    ///   Pano_Cot         -> PanelCode ("paño")
    ///   Veces_Cot        -> Times (repeticiones del paño)
    ///   LongitudInt_Cot  -> InnerLength
    ///   LongitudExt_Cot  -> OuterLength
    ///   Apoyo_Cot        -> Support
    ///   Cantidad_Cot     -> Quantity
    ///   TotalMtl_Cot     -> TotalMaterial
    ///   NBovedilla_Cot   -> VaultCount
    ///   PDesperdicio_Cot -> WastePercentage
    ///   Fila_Cot         -> Row
    ///   CantidadB_Cot    -> QuantityB (ver nota "B" en CotizacionMetradoResumen)
    ///   Apoyo2_Cot       -> Support2
    ///   Ancho_Pre        -> Width
    ///   Area_Pre         -> Area
    ///   Precio_Met       -> MaterialPrice
    ///   Igv_Met          -> MaterialIgv
    ///   Precio_Tra       -> TransportPrice
    ///   Precio_Bov       -> VaultPrice
    ///   Precio_BovT      -> VaultTotalPrice
    ///   Precio_MetD, Precio_TraD, Precio_BovD, Precio_BovTD -> variantes
    ///                     "Alt" de los 4 precios anteriores (mismo caso del
    ///                     sufijo "D" de CotizacionMetradoResumen)
    ///   Ordena_cot       -> SortOrder (char(3), visualización; distinto de
    ///                     Order/Orden_Cot que es parte de la PK)
    ///   TotalMtl_EntCot  -> DeliveredTotalMaterial
    ///   CantidadB_EntCot -> DeliveredQuantityB
    ///   Precio_Pol, Precio_PolT, Precio_PolD, Precio_PolTD -> variante en
    ///                     poliestireno de VaultPrice/VaultTotalPrice
    ///                     (PolystyrenePrice, PolystyreneTotalPrice y sus
    ///                     variantes "Alt")
    ///   Ensanche_Met     -> Widening
    ///   ApoyoP_Cot       -> SupportP (variante poliestireno de Support)
    ///   PDesperdicioP_Cot -> WastePercentageP
    ///   CantidadP_Cot    -> QuantityP
    ///   Anclaje_Cot      -> Anchorage (enum TipoAnclaje: ver esa clase,
    ///                     CORREGIDO de un bool HasAnchorage anterior:
    ///                     son 3 valores S/N/L, no 2)
    ///   Espaciamiento_Cot -> Spacing
    /// </summary>
    public class CotizacionMetradoDetalle : IAuditableEntity
    {
        public string NegocioCode { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public string Code { get; set; } = default!;
        public int LevelNumber { get; set; }
        public CotizacionMetradoResumen MetradoResumen { get; set; } = default!;

        /// <summary>Correlativo de orden dentro del nivel (char(3) en el legacy, parte de la PK).</summary>
        public string Order { get; set; } = default!;

        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public Producto Producto { get; set; } = default!;

        public string PanelCode { get; set; } = default!;
        public int Times { get; set; }
        public decimal InnerLength { get; set; }
        public decimal OuterLength { get; set; }
        public decimal Support { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalMaterial { get; set; }
        public decimal VaultCount { get; set; }
        public decimal WastePercentage { get; set; }
        public decimal Row { get; set; }
        public decimal QuantityB { get; set; }
        public decimal Support2 { get; set; }
        public decimal Width { get; set; }
        public decimal Area { get; set; }

        public decimal MaterialPrice { get; set; }
        public decimal MaterialIgv { get; set; }
        public decimal TransportPrice { get; set; }
        public decimal VaultPrice { get; set; }
        public decimal VaultTotalPrice { get; set; }

        public decimal MaterialPriceAlt { get; set; }
        public decimal TransportPriceAlt { get; set; }
        public decimal VaultPriceAlt { get; set; }
        public decimal VaultTotalPriceAlt { get; set; }

        public string SortOrder { get; set; } = default!;

        public decimal DeliveredTotalMaterial { get; set; }
        public decimal DeliveredQuantityB { get; set; }

        public decimal PolystyrenePrice { get; set; }
        public decimal PolystyreneTotalPrice { get; set; }
        public decimal PolystyrenePriceAlt { get; set; }
        public decimal PolystyreneTotalPriceAlt { get; set; }

        public decimal Widening { get; set; }
        public decimal SupportP { get; set; }
        public decimal WastePercentageP { get; set; }
        public decimal QuantityP { get; set; }

        public TipoAnclaje Anchorage { get; set; } = TipoAnclaje.Si;
        public decimal Spacing { get; set; }

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
