namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;

/// <summary>
/// Línea manual del vale. El precio lo resuelve el servidor (precio promedio vigente del artículo en la
/// planta) y el total y el ítem también.
/// </summary>
public sealed class ValeDetalleRequest
{
    public string ArticuloCode { get; set; } = default!;
    public decimal Cantidad { get; set; }

    /// <summary>Horómetro; solo aplica (y es obligatorio) si el artículo lo exige.</summary>
    public string? Propiedad01 { get; set; }
}

/// <summary>
/// Material pendiente (estado 01) de la Orden de Trabajo del vale que se asigna al crearlo.
/// Cantidad y costo salen del material; queda asignado (15) hasta procesar, devolver o anular.
/// </summary>
public sealed class ValeMaterialOtRequest
{
    public string ArticuloCode { get; set; } = default!;
    public DateTime FechaProceso { get; set; }
}
