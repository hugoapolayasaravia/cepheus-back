using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;

/// <summary>Línea de la nota cuyo saldo no alcanza para generar el vale.</summary>
public sealed record LineaSinStock(
    int ItemNumber,
    string ArticuloCode,
    string ArticuloName,
    string UnidadMedidaCode,
    decimal Cantidad,
    decimal Stock,
    decimal Pendiente);

public static class NotaIngresoValeRules
{
    /// <summary>
    /// Logi_sp_Consulta_Stock_NI_x_Vales (dw_con_stocks_ni_negativos_vales): líneas de la nota donde
    /// Cantidad &gt; Stock − Pendiente, con Stock = fn_Stock_Articulo (stock de la planta) y Pendiente =
    /// fn_Stock_Pendiente (lo reservado por vales y ajustes pendientes). Se evalúa línea por línea, como el legacy.
    /// </summary>
    public static async Task<IReadOnlyList<LineaSinStock>> GetLineasSinStockAsync(
        IUnitOfWork uow,
        string plantaCode,
        IEnumerable<NotaIngresoDetalle> lineas,
        CancellationToken ct)
    {
        var resultado = new List<LineaSinStock>();

        foreach (var linea in lineas.OrderBy(l => l.ItemNumber))
        {
            var stock = await uow.Logistica.Maestros.StockArticulos.Query().AsNoTracking()
                .Where(s => s.PlantaCode == plantaCode && s.ArticuloCode == linea.ArticuloCode)
                .Select(s => (decimal?)s.Quantity)
                .FirstOrDefaultAsync(ct) ?? 0m;

            var pendiente = await ValeRules.GetReservedAsync(uow, plantaCode, linea.ArticuloCode, ct);

            if (linea.Cantidad <= stock - pendiente)
                continue;

            var articulo = await uow.Logistica.Maestros.Articulos.Query().AsNoTracking()
                .Where(a => a.Code == linea.ArticuloCode)
                .Select(a => new { a.Name, a.UnidadMedidaCode })
                .FirstOrDefaultAsync(ct);

            resultado.Add(new LineaSinStock(
                linea.ItemNumber, linea.ArticuloCode,
                articulo?.Name ?? string.Empty, articulo?.UnidadMedidaCode ?? string.Empty,
                linea.Cantidad, stock, pendiente));
        }

        return resultado;
    }
}
