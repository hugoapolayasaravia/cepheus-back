namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionMetradoDetalles.Common
{
    public class CotizacionMetradoDetalleResponse
    {
        public string NegocioCode { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public string Code { get; set; } = default!;
        public int LevelNumber { get; set; }
        public string Order { get; set; } = default!;

        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;

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

        public string Anchorage { get; set; } = default!;
        public decimal Spacing { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
