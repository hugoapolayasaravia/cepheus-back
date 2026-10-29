using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.ProcesarAjusteInventario;

/// <summary>Stock, líneas y estado del ajuste se confirman con UN solo SaveChanges.</summary>
public sealed class ProcesarAjusteInventarioCommandHandler
    : IRequestHandler<ProcesarAjusteInventarioCommand, AjusteInventarioResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IArticuloStockMovementService _stock;

    public ProcesarAjusteInventarioCommandHandler(
        IUnitOfWork uow,
        IArticuloStockMovementService stock)
    {
        _uow = uow;
        _stock = stock;
    }

    public async Task<AjusteInventarioResponse> Handle(
        ProcesarAjusteInventarioCommand request,
        CancellationToken ct)
    {
        var planta = AjusteInventarioRules.Normalize(request.PlantaCode);
        var code = AjusteInventarioRules.Normalize(request.Code);

        var ajuste = await AjusteInventarioReader.LoadForUpdateAsync(_uow, planta, code, ct);

        if (ajuste.Estado is not (EstadoAjusteInventario.Pendiente or EstadoAjusteInventario.EntregaParcial))
        {
            throw new InvalidOperationException(
                $"El Ajuste de Inventario se encuentra {ajuste.Estado}; solo se procesa desde Pendiente o Entrega Parcial.");
        }

        await AjusteInventarioRules.EnsurePeriodOpenAsync(
            _uow,
            planta,
            ajuste.FechaEntrega,
            "La Fecha de Entrega no puede ser menor o igual al cierre del período.",
            ct);

        // Solo las líneas pendientes: no se vuelve a aplicar lo ya procesado.
        var pendientes = ajuste.Detalles
            .Where(d => d.Estado == EstadoAjusteInventarioDetalle.Pendiente)
            .ToList();

        if (pendientes.Count == 0)
        {
            throw new InvalidOperationException(
                "El Ajuste de Inventario no tiene líneas pendientes por procesar.");
        }

        var seleccion = pendientes;

        if (request.Articulos is { Count: > 0 })
        {
            var solicitados = request.Articulos
                .Select(AjusteInventarioRules.Normalize)
                .Distinct()
                .ToList();

            var noPendientes = solicitados
                .Where(a => pendientes.All(p => p.ArticuloCode != a))
                .ToList();

            if (noPendientes.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Los artículos {string.Join(", ", noPendientes)} no son líneas pendientes del ajuste.");
            }

            seleccion = pendientes
                .Where(p => solicitados.Contains(p.ArticuloCode))
                .ToList();
        }

        await EnsureSaldoSuficienteAsync(
            planta,
            seleccion.Where(l => l.Tipo == TipoAjusteInventario.Faltante).ToList(),
            "al procesar el ajuste",
            ct);

        foreach (var linea in seleccion.OrderBy(l => l.ItemNumber))
        {
            var esSobrante = linea.Tipo == TipoAjusteInventario.Sobrante;

            // Logi_sp_Actualiza_Stock_Planta_Articulo con Calculo 'N': solo cambia la cantidad.
            // Kardex: el sobrante entra al precio del ajuste (el cierre lo promedia, como el legacy);
            // el faltante sale al costo promedio vigente (el cierre lo revaloriza).
            await _stock.ApplyAsync(
                planta,
                linea.ArticuloCode,
                linea.Cantidad,
                linea.Precio,
                0m,
                0m,
                increment: esSobrante,
                recalculateAverage: false,
                context: new StockMovementContext(
                    esSobrante ? OrigenMovimientoArticulo.AjusteIngreso : OrigenMovimientoArticulo.AjusteSalida,
                    ajuste.Code,
                    linea.ItemNumber,
                    ajuste.FechaEntrega,
                    CostoReferencia: esSobrante ? linea.Precio : null),
                cancellationToken: ct);

            linea.Estado = EstadoAjusteInventarioDetalle.Procesado;
        }

        var quedanPendientes = ajuste.Detalles
            .Any(d => d.Estado == EstadoAjusteInventarioDetalle.Pendiente);

        ajuste.Estado = quedanPendientes
            ? EstadoAjusteInventario.EntregaParcial
            : EstadoAjusteInventario.Procesado;

        await AjusteInventarioReader.SaveAsync(_uow, ct);

        return await AjusteInventarioReader.GetAsync(_uow, planta, code, ct);
    }

    /// <summary>
    /// Logi_sp_Consulta_Stock_Ajuste: lista los artículos que dejarían saldo negativo
    /// (ítem, artículo, cantidad, stock y saldo) antes de tocar nada.
    /// </summary>
    private async Task EnsureSaldoSuficienteAsync(
        string planta,
        List<AjusteInventarioDetalle> lineas,
        string contexto,
        CancellationToken ct)
    {
        if (lineas.Count == 0)
        {
            return;
        }

        var codigos = lineas.Select(l => l.ArticuloCode).ToList();

        var stocks = await _uow.Logistica.Maestros.StockArticulos
            .Query()
            .AsNoTracking()
            .Where(s => s.PlantaCode == planta && codigos.Contains(s.ArticuloCode))
            .ToDictionaryAsync(s => s.ArticuloCode, s => s.Quantity, ct);

        var faltantes = lineas
            .Select(l =>
            {
                var stock = stocks.GetValueOrDefault(l.ArticuloCode);
                return new { l.ItemNumber, l.ArticuloCode, l.Cantidad, Stock = stock, Saldo = stock - l.Cantidad };
            })
            .Where(x => x.Saldo < 0)
            .OrderBy(x => x.ItemNumber)
            .ToList();

        if (faltantes.Count == 0)
        {
            return;
        }

        var detalle = string.Join(
            "; ",
            faltantes.Select(f =>
                $"ítem {f.ItemNumber} {f.ArticuloCode}: cantidad {f.Cantidad}, stock {f.Stock}, saldo {f.Saldo}"));

        throw new InvalidOperationException(
            $"Existen artículos que {contexto} dejarían saldo negativo: {detalle}.");
    }
}
