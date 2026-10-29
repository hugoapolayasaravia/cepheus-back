using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.AjustesInventario.DevolverAjusteInventario;

public sealed class DevolverAjusteInventarioCommandHandler
    : IRequestHandler<DevolverAjusteInventarioCommand, AjusteInventarioResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IArticuloStockMovementService _stock;

    public DevolverAjusteInventarioCommandHandler(
        IUnitOfWork uow,
        IArticuloStockMovementService stock)
    {
        _uow = uow;
        _stock = stock;
    }

    public async Task<AjusteInventarioResponse> Handle(
        DevolverAjusteInventarioCommand request,
        CancellationToken ct)
    {
        var planta = AjusteInventarioRules.Normalize(request.PlantaCode);
        var code = AjusteInventarioRules.Normalize(request.Code);

        var ajuste = await AjusteInventarioReader.LoadForUpdateAsync(_uow, planta, code, ct);

        if (ajuste.Estado is not (EstadoAjusteInventario.Procesado or EstadoAjusteInventario.EntregaParcial))
        {
            throw new InvalidOperationException(
                $"El Ajuste de Inventario se encuentra {ajuste.Estado}; solo se devuelve desde Procesado o Entrega Parcial.");
        }

        if (!string.IsNullOrWhiteSpace(ajuste.AsientoContable))
        {
            throw new InvalidOperationException(
                "El Ajuste de Inventario ya generó asiento contable. No se puede hacer una devolución.");
        }

        await AjusteInventarioRules.EnsurePeriodOpenAsync(
            _uow,
            planta,
            ajuste.FechaEntrega,
            "El Ajuste de Inventario está procesado y su período está cerrado. Verifique el cierre.",
            ct);

        // Solo lo procesado se puede devolver (no pendientes, anuladas ni ya devueltas).
        var procesadas = ajuste.Detalles
            .Where(d => d.Estado == EstadoAjusteInventarioDetalle.Procesado)
            .ToList();

        if (procesadas.Count == 0)
        {
            throw new InvalidOperationException(
                "El Ajuste de Inventario no tiene líneas procesadas para devolver.");
        }

        var seleccion = procesadas;

        if (request.Articulos is { Count: > 0 })
        {
            var solicitados = request.Articulos
                .Select(AjusteInventarioRules.Normalize)
                .Distinct()
                .ToList();

            var noProcesadas = solicitados
                .Where(a => procesadas.All(p => p.ArticuloCode != a))
                .ToList();

            if (noProcesadas.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Los artículos {string.Join(", ", noProcesadas)} no son líneas procesadas del ajuste.");
            }

            seleccion = procesadas
                .Where(p => solicitados.Contains(p.ArticuloCode))
                .ToList();
        }

        await EnsureSaldoSuficienteAsync(
            planta,
            seleccion.Where(l => l.Tipo == TipoAjusteInventario.Sobrante).ToList(),
            ct);

        foreach (var linea in seleccion.OrderBy(l => l.ItemNumber))
        {
            // Reversa: un sobrante había sumado (ahora resta) y un faltante había restado (ahora suma).
            var eraSobrante = linea.Tipo == TipoAjusteInventario.Sobrante;

            // Kardex: deshace el movimiento original de la línea (mismo ajuste, ítem y origen).
            await _stock.ApplyAsync(
                planta,
                linea.ArticuloCode,
                linea.Cantidad,
                0m,
                0m,
                0m,
                increment: !eraSobrante,
                recalculateAverage: false,
                context: new StockMovementContext(
                    eraSobrante ? OrigenMovimientoArticulo.AjusteIngreso : OrigenMovimientoArticulo.AjusteSalida,
                    ajuste.Code,
                    linea.ItemNumber,
                    ajuste.FechaEntrega,
                    EsReversion: true),
                cancellationToken: ct);

            linea.Estado = EstadoAjusteInventarioDetalle.Devuelto;
        }

        var quedanProcesadas = ajuste.Detalles
            .Any(d => d.Estado == EstadoAjusteInventarioDetalle.Procesado);
        var quedanPendientes = ajuste.Detalles
            .Any(d => d.Estado == EstadoAjusteInventarioDetalle.Pendiente);

        if (!quedanProcesadas && !quedanPendientes)
        {
            ajuste.Estado = EstadoAjusteInventario.Devuelto;
        }
        else
        {
            // Devolución parcial: el ajuste queda con entregas parciales y los importes excluyen lo devuelto.
            ajuste.Estado = EstadoAjusteInventario.EntregaParcial;
            await AjusteInventarioRules.RecalculateTotalsAsync(_uow, ajuste, ct);
        }

        await AjusteInventarioReader.SaveAsync(_uow, ct);

        return await AjusteInventarioReader.GetAsync(_uow, planta, code, ct);
    }

    /// <summary>
    /// Logi_sp_Consulta_Stock_Ajuste (tipo 'I'): devolver un sobrante resta stock; si no alcanza, se lista
    /// (ítem, artículo, cantidad, stock y saldo) y no se toca nada.
    /// </summary>
    private async Task EnsureSaldoSuficienteAsync(
        string planta,
        List<AjusteInventarioDetalle> lineas,
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
            $"Existen artículos que al devolver el ajuste dejarían saldo negativo: {detalle}.");
    }
}
