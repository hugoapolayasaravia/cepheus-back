namespace Cepheus.Application.Features.Facturacion.Transacciones.NotasDebitoVenta.Common
{
    /// <summary>
    /// Línea de la nota de débito. El total de la línea es cantidad x precio (en la moneda del documento).
    /// Si no se indica descripción se usa el nombre del producto (como el legacy al elegir el producto).
    /// </summary>
    public record NotaDebitoVentaDetalleInput(
        string TipoProductoCode,
        string ProductoCode,
        string? Description,
        decimal Quantity,
        decimal UnitPrice,
        int? LineKey
    );
}
