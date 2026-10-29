using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;

/// <summary>Resultado de validar una línea manual (precio, horómetro y artículo resueltos por el servidor).</summary>
public sealed record ValeLineaResuelta(
    Articulo Articulo,
    decimal Precio,
    string? Propiedad01);

/// <summary>
/// Validaciones de una línea manual del vale (PB: ue_set_btn_grb_data, caso "Art", y ue_busqueda de dw_6).
/// </summary>
public static class ValeLineas
{
    /// <summary>
    /// Línea nueva (precioExistente = null): el artículo debe estar permitido para el tipo de vale y el precio
    /// sale del stock de la planta. Línea existente (precioExistente informado): conserva su precio y no
    /// revalida el tipo de vale, como el PB al modificar. En ambos casos la cantidad no puede superar el
    /// disponible (stock - reservado, sin contar la cantidad que la propia línea ya tenía).
    /// exigirPropiedad = false para los materiales que vienen de la OT: el PB no exige horómetro en ese flujo.
    /// </summary>
    public static async Task<ValeLineaResuelta> ResolverAsync(
        IUnitOfWork uow,
        string plantaCode,
        string tipoValeCode,
        string articuloCode,
        decimal cantidad,
        string? propiedad01,
        decimal cantidadActual,
        decimal? precioExistente,
        bool exigirPropiedad,
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

        if (articulo is null || !articulo.IsActive)
        {
            throw new InvalidOperationException(
                $"El artículo {articuloCode} no existe o está inactivo.");
        }

        //if (precioExistente is null)
        //{
        //    // fn_TipoVale_Articulo: el artículo debe estar permitido para el tipo de vale.
        //    var permitido = await uow.Logistica.Catalogos.TiposValeArticulo
        //        .Query()
        //        .AsNoTracking()
        //        .AnyAsync(
        //            x => x.TipoValeCode == tipoValeCode &&
        //                 x.ArticuloCode == articuloCode,
        //            ct);

        //    if (!permitido)
        //    {
        //        throw new InvalidOperationException(
        //            $"El artículo {articuloCode} no coincide con el tipo de vale {tipoValeCode}. Revisar la relación Tipos Vales Articulo");
        //    }
        //}

        var stock = await uow.Logistica.Maestros.StockArticulos
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(
                s => s.PlantaCode == plantaCode &&
                     s.ArticuloCode == articuloCode,
                ct);

        // PRECIOARTICULO: precio promedio del stock de la planta.
        var precio = precioExistente ?? stock?.AverageCost ?? 0m;

        if (precio <= 0)
        {
            throw new InvalidOperationException(
                $"El artículo {articuloCode} no tiene precio registrado en la planta {plantaCode}.");
        }

        var reservado = await ValeRules.GetReservedAsync(uow, plantaCode, articuloCode, ct);
        var disponible = (stock?.Quantity ?? 0m) - (reservado - cantidadActual);

        if (cantidad > disponible)
        {
            throw new InvalidOperationException(
                $"La cantidad del artículo {articuloCode} ({cantidad}) supera el saldo disponible ({disponible}).");
        }

        // Propiedad (horómetro): obligatoria si el artículo la exige; si no, se descarta.
        string? propiedad = null;

        if (articulo.RequiresHorometro && exigirPropiedad)
        {
            propiedad = propiedad01?.Trim();

            if (string.IsNullOrWhiteSpace(propiedad))
            {
                throw new InvalidOperationException(
                    $"El artículo {articuloCode} requiere el dato de horómetro.");
            }

            if (propiedad.Length > 7)
            {
                throw new InvalidOperationException(
                    "El horómetro no puede superar los 7 caracteres.");
            }
        }

        return new ValeLineaResuelta(articulo, precio, propiedad);
    }

    /// <summary>Renumera los ítems 1..n por código de artículo (Logi_sp_Actualiza_Item_Vales).</summary>
    public static void Renumerar(Vale vale)
    {
        var item = 1;

        foreach (var detalle in vale.Detalles.OrderBy(d => d.ArticuloCode, StringComparer.Ordinal))
        {
            detalle.ItemNumber = item++;
        }
    }
}
