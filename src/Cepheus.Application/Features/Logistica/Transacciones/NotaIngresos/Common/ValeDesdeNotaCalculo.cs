namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;

/// <summary>Línea de la Nota de Ingreso con su precio ya en soles.</summary>
public sealed record LineaNota(string ArticuloCode, decimal Cantidad, decimal PrecioSoles);

/// <summary>Línea del vale que se genera.</summary>
public sealed record LineaVale(string ArticuloCode, decimal Cantidad, decimal Precio);

/// <summary>
/// Cálculo (sin base de datos) de las líneas del vale que el botón "Vales" de la Nota de Ingreso genera (ue_vale):
///   - precio: el de la línea de la nota; si la nota está en dólares y hay tipo de cambio, ROUND(precio × tipo cambio, 6).
///     El legacy NO aplica el descuento de la línea;
///   - cantidad: la de la línea; total = ROUND(precio × cantidad, 2) (lo calcula el vale).
/// El vale no admite un artículo repetido. El legacy (Logi_sp_AgregaMValesDet) graba por artículo, así que si la nota
/// repetía un artículo quedaba solo la última línea; aquí se suman las cantidades y el precio es el promedio ponderado.
/// </summary>
public static class ValeDesdeNotaCalculo
{
    public static decimal PrecioSoles(decimal precio, bool esDolar, decimal tipoCambio)
        => esDolar && tipoCambio > 0
            ? Math.Round(precio * tipoCambio, 6, MidpointRounding.AwayFromZero)
            : precio;

    public static IReadOnlyList<LineaVale> AgruparPorArticulo(IEnumerable<LineaNota> lineas)
    {
        return lineas
            .GroupBy(l => l.ArticuloCode)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .Select(g =>
            {
                var cantidad = g.Sum(l => l.Cantidad);
                var precio = g.Count() == 1
                    ? g.First().PrecioSoles
                    : Math.Round(g.Sum(l => l.PrecioSoles * l.Cantidad) / cantidad, 6, MidpointRounding.AwayFromZero);

                return new LineaVale(g.Key, cantidad, precio);
            })
            .ToList();
    }
}
