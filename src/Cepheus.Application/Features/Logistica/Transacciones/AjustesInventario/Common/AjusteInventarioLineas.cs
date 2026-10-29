using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;

/// <summary>Línea ya validada: artículo y precio resueltos por el servidor.</summary>
public sealed record AjusteLineaResuelta(
    Articulo Articulo,
    decimal Precio);

/// <summary>
/// Validaciones de una línea del ajuste (PB: ue_set_btn_grb_data, caso nuevo/modificación, y ue_busqueda de dw_6).
/// El precio NO lo digita el usuario: es el precio promedio vigente del artículo en la planta.
/// La validación contra el disponible está comentada en el PB, así que solo se controla el saldo al procesar.
/// </summary>
public static class AjusteInventarioLineas
{
    /// <summary>
    /// precioExistente informado = línea que ya existe: conserva su precio (el PB no lo vuelve a consultar al
    /// modificar). Si es null se resuelve el precio promedio del stock de la planta.
    /// </summary>
    public static async Task<AjusteLineaResuelta> ResolverAsync(
        IUnitOfWork uow,
        string plantaCode,
        string articuloCode,
        decimal cantidad,
        decimal? precioExistente,
        CancellationToken ct)
    {
        if (cantidad <= 0)
        {
            throw new InvalidOperationException(
                $"La cantidad del artículo {articuloCode} debe ser mayor a cero.");
        }

        var articulo = await uow.Logistica.Maestros.Articulos
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Code == articuloCode, ct);

        if (articulo is null)
        {
            throw new InvalidOperationException($"El artículo {articuloCode} no existe.");
        }

        var precio = precioExistente;

        if (precio is null)
        {
            // PRECIOARTICULO: precio promedio del stock de la planta.
            var stock = await uow.Logistica.Maestros.StockArticulos
                .Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    s => s.PlantaCode == plantaCode &&
                         s.ArticuloCode == articuloCode,
                    ct);

            precio = stock?.AverageCost ?? 0m;
        }

        if (precio <= 0)
        {
            throw new InvalidOperationException(
                $"El artículo {articuloCode} no tiene precio registrado en la planta {plantaCode}.");
        }

        return new AjusteLineaResuelta(articulo, precio.Value);
    }

    /// <summary>Renumera los ítems 1..n por código de artículo (Logi_sp_Actualiza_Item_Ajustes).</summary>
    public static void Renumerar(AjusteInventario ajuste)
    {
        var item = 1;

        foreach (var detalle in ajuste.Detalles.OrderBy(d => d.ArticuloCode, StringComparer.Ordinal))
        {
            detalle.ItemNumber = item++;
        }
    }
}
