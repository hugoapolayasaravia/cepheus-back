using Cepheus.Domain.Comun;
using Cepheus.Domain.Facturacion.Catalogos;

namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Resumen de metrado por nivel/piso de una Cotizacion (losas aligeradas):
    /// totales de bovedillas, metros y precios calculados para ese nivel.
    /// Hijo del aggregate Cotizacion; agrupa a su vez el detalle de metrado
    /// (CotizacionMetradoDetalle).
    ///
    /// Legacy: dbo.CotizacionesMetradoRes (SQL Server). PK compuesta
    /// (Codigo_Neg, Ano_cot, Mes_cot, Codigo_Cot, Numero_Niv).
    ///
    /// Mapeo de columnas legacy -> propiedades profesionales:
    ///   Codigo_Neg, Ano_cot, Mes_cot, Codigo_Cot -> FK compuesta -> Cotizacion
    ///   Numero_Niv     -> LevelNumber (parte de la PK)
    ///   Nivel_Cot      -> LevelName
    ///   Codigo_Alt     -> AlturaLosaCode (FK -> AlturaLosa)
    ///   SobraCarga     -> OverloadOrShortage (PENDIENTE CONFIRMAR significado
    ///                     exacto: sin comentario en el legacy; se conserva el
    ///                     nombre literal traducido)
    ///   MLineal_Vig    -> LinealMeters
    ///   Total_Bov      -> TotalVaults ("bovedillas")
    ///   Total_Mts      -> TotalMeters
    ///   Total_Pre      -> TotalPrice
    ///   Precio_M2      -> PricePerM2
    ///   Cantidad_Cot   -> Quantity
    ///   Edificio_Niv   -> BuildingLevel
    ///   Transporte_Niv -> HasTransport (char(1) S/N -> bool)
    ///   Total_BovD, Total_MtsD, Total_PreD, Precio_M2D -> TotalVaultsAlt,
    ///                     TotalMetersAlt, TotalPriceAlt, PricePerM2Alt.
    ///                     PENDIENTE CONFIRMAR: sufijo "D" sin documentar en
    ///                     el legacy — se asume una segunda variante/moneda
    ///                     de los mismos totales (posiblemente dólares);
    ///                     confirmar antes de escribir el ETL.
    ///   PrecioMinimo   -> HasMinPrice (char(1) S/N -> bool)
    ///   Total_Min, Total_MinD, Total_MinB, Total_MinBD -> MinTotal,
    ///                     MinTotalAlt, MinTotalB, MinTotalAltB. PENDIENTE
    ///                     CONFIRMAR el significado del sufijo "B" (mismo
    ///                     caso que "D"); se preserva el patrón de nombres
    ///                     hasta poder confirmarlo contigo.
    ///   TransporteB_Niv -> HasTransportB (bool, ver nota "B" arriba)
    ///   PrecioMinimoB   -> HasMinPriceB (bool, ver nota "B" arriba)
    /// </summary>
    public class CotizacionMetradoResumen : IAuditableEntity
    {
        public string NegocioCode { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public string Code { get; set; } = default!;
        public Cotizacion Cotizacion { get; set; } = default!;

        public int LevelNumber { get; set; }

        public string LevelName { get; set; } = default!;

        public string AlturaLosaCode { get; set; } = default!;
        public AlturaLosa AlturaLosa { get; set; } = default!;

        /// <summary>Ver nota PENDIENTE CONFIRMAR en el comentario de la clase.</summary>
        public string OverloadOrShortage { get; set; } = default!;

        public decimal LinealMeters { get; set; }
        public decimal TotalVaults { get; set; }
        public decimal TotalMeters { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PricePerM2 { get; set; }
        public decimal Quantity { get; set; }
        public string BuildingLevel { get; set; } = default!;
        public bool HasTransport { get; set; } = true;

        /// <summary>Ver nota PENDIENTE CONFIRMAR (sufijo "D") en el comentario de la clase.</summary>
        public decimal TotalVaultsAlt { get; set; }
        public decimal TotalMetersAlt { get; set; }
        public decimal TotalPriceAlt { get; set; }
        public decimal PricePerM2Alt { get; set; }

        public bool HasMinPrice { get; set; }
        public decimal MinTotal { get; set; }
        public decimal MinTotalAlt { get; set; }

        /// <summary>Ver nota PENDIENTE CONFIRMAR (sufijo "B") en el comentario de la clase.</summary>
        public decimal MinTotalB { get; set; }
        public decimal MinTotalAltB { get; set; }
        public bool HasTransportB { get; set; } = true;
        public bool HasMinPriceB { get; set; }

        public ICollection<CotizacionMetradoDetalle> Detalles { get; set; } = new List<CotizacionMetradoDetalle>();

        // Auditoría (IAuditableEntity)
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        // Concurrencia optimista
        public byte[] RowVersion { get; set; } = default!;
    }
}
