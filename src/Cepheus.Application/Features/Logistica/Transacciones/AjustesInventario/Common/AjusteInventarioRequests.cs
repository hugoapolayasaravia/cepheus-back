using Cepheus.Domain.Logistica.Enum;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;

/// <summary>
/// Línea del ajuste. El precio lo resuelve el servidor (precio promedio vigente del artículo en la planta);
/// el total y el ítem también.
/// </summary>
public sealed class AjusteInventarioDetalleRequest
{
    public string ArticuloCode { get; set; } = default!;
    public TipoAjusteInventario Tipo { get; set; }
    public decimal Cantidad { get; set; }
}
