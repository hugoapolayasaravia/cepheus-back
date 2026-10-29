namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoResumenes.Common
{
    /// <summary>
    /// Nota general del módulo de Metrado (Resumen y Detalle): esta entrega
    /// cubre la PERSISTENCIA de los valores de metrado (crear/editar/eliminar
    /// nivel y línea, tal como llegan calculados desde el cliente), no el
    /// CÁLCULO de ingeniería en sí (fórmulas de bovedillas, desperdicio,
    /// paños, etc.), que no está documentado en el DDL legacy y requiere
    /// definirlo contigo antes de automatizarlo en el backend. Por ahora los
    /// Create/Update reciben los valores ya calculados (por Excel, por la UI,
    /// o por el motor que definan) y solo los validan/persisten.
    /// </summary>
    public class CotizacionMetradoResumenResponse
    {
        public string NegocioCode { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public string Code { get; set; } = default!;
        public int LevelNumber { get; set; }

        public string LevelName { get; set; } = default!;
        public string AlturaLosaCode { get; set; } = default!;
        public string OverloadOrShortage { get; set; } = default!;

        public decimal LinealMeters { get; set; }
        public decimal TotalVaults { get; set; }
        public decimal TotalMeters { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal PricePerM2 { get; set; }
        public decimal Quantity { get; set; }
        public string BuildingLevel { get; set; } = default!;
        public bool HasTransport { get; set; }

        public decimal TotalVaultsAlt { get; set; }
        public decimal TotalMetersAlt { get; set; }
        public decimal TotalPriceAlt { get; set; }
        public decimal PricePerM2Alt { get; set; }

        public bool HasMinPrice { get; set; }
        public decimal MinTotal { get; set; }
        public decimal MinTotalAlt { get; set; }
        public decimal MinTotalB { get; set; }
        public decimal MinTotalAltB { get; set; }
        public bool HasTransportB { get; set; }
        public bool HasMinPriceB { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
