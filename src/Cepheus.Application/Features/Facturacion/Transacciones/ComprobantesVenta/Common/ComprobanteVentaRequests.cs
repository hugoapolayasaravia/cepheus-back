namespace Cepheus.Application.Features.Facturacion.Transacciones.ComprobantesVenta.Common;

public sealed class ComprobanteVentaLineaLibreRequest
{
    public string ProductoTipoCode { get; set; } = default!;
    public string ProductoCode { get; set; } = default!;
    public decimal Quantity { get; set; }

    /// <summary>Precio unitario en la moneda del comprobante (sin IGV).</summary>
    public decimal UnitPrice { get; set; }
}
