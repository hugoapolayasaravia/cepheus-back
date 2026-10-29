namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;

/// <summary>
/// Línea enviada al registrar una Nota de Ingreso. El total y el correlativo de ítem los calcula el servidor.
/// Descuento es un PORCENTAJE (0-100), igual que el legacy.
/// En Nota de Crédito, PedidoCode identifica la línea de la NI referenciada; el precio y el descuento solo
/// se toman del request cuando el motivo es 01 (ajuste).
/// </summary>
public sealed class NotaIngresoDetalleRequest
{
    public string ArticuloCode { get; set; } = default!;
    public string? PedidoCode { get; set; }
    public decimal Cantidad { get; set; }
    public decimal Precio { get; set; }
    public decimal Descuento { get; set; }
}
