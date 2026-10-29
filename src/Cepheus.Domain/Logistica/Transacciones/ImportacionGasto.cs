// Cepheus.Domain/Logistica/Transacciones/ImportacionGasto.cs
using Cepheus.Domain.Comun;
using Cepheus.Domain.Comunes;
using Cepheus.Domain.Logistica.Maestros;

namespace Cepheus.Domain.Logistica.Transacciones
{
    /// <summary>
    /// Gasto administrativo de una Importación (agente de aduanas, flete local, etc.).
    /// Legacy: dbo.MImportacionGDet. PK: (Planta, Importación, Proveedor, Número de documento).
    ///
    /// Cambios frente al legacy:
    ///   - Codigo_tdo -> ComprobantePagoCode FK Comunes.ComprobantePago
    ///   - Moneda char(1) ('S'/'D') -> MonedaCode FK Comunes.Moneda (ISO: 'S' = PEN, 'D' = USD)
    ///   - Afecto 'S'/'N' -> bool Afecto
    ///   - T_cambio -> TipoCambio: venta de Comunes.TipoCambio en FechaEmision (calculado)
    ///   - Neto_Gas / Neto_GasI -> NetoGasto (afecto) / NetoGastoInafecto
    ///   - Igv_Gas / Igv_Ext -> Igv / IgvExterior. Si el comprobante AffectsForeignIgv
    ///     (legacy: tipo de documento '91'), el IGV calculado va a IgvExterior y Igv queda en 0.
    ///   - Total_Gas -> Total = NetoGasto + NetoGastoInafecto + Igv + IgvExterior
    /// </summary>
    public class ImportacionGasto : IAuditableEntity
    {
        public string PlantaCode { get; set; } = default!;
        public string ImportacionCode { get; set; } = default!;
        public Importacion Importacion { get; set; } = default!;

        public string ProveedorCode { get; set; } = default!;
        public Proveedor Proveedor { get; set; } = default!;

        public string NumeroDocumento { get; set; } = default!;

        public string ComprobantePagoCode { get; set; } = default!;
        public ComprobantePago ComprobantePago { get; set; } = default!;

        public string MonedaCode { get; set; } = default!;
        public Moneda Moneda { get; set; } = default!;

        public bool Afecto { get; set; }
        public DateTime FechaEmision { get; set; }
        public decimal TipoCambio { get; set; }

        public decimal NetoGasto { get; set; }
        public decimal NetoGastoInafecto { get; set; }
        public decimal Igv { get; set; }
        public decimal IgvExterior { get; set; }
        public decimal Total { get; set; }

        /// <summary>Prorrateo del gasto por artículo (calculado). Legacy: MImportacionGDet_Articulos.</summary>
        public ICollection<ImportacionGastoArticulo> Articulos { get; set; } = new List<ImportacionGastoArticulo>();

        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }

        public byte[] RowVersion { get; set; } = default!;
    }
}
