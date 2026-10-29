namespace Cepheus.Application.Features.Facturacion.Transacciones.CotizacionDetalles.Common
{
    public class CotizacionDetalleResponse
    {
        public string NegocioCode { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public string Code { get; set; } = default!;
        public int Item { get; set; }

        public string ProductoTipoCode { get; set; } = default!;
        public string ProductoCode { get; set; } = default!;
        public string UnitCode { get; set; } = default!;

        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }

        public string Observations { get; set; } = default!;
        public int Order { get; set; }
        public decimal DeliveredQuantity { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public byte[] RowVersion { get; set; } = default!;
    }
}
