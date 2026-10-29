namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasCreditoVenta.Common
{
    public record NotaCreditoVentaDetalleInput(
        string TipoProductoCode,
        string ProductoCode,
        string? GuideNumber,
        string? Description,
        decimal Quantity,
        decimal UnitPrice,
        int? LineKey,
        string? OrderCode
    );
}
