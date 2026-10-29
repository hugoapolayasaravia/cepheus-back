using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Vales.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Vales.ProcesarVale;

/// <summary>
/// Stock, líneas, consumo de la OT y estado del vale se confirman con UN solo SaveChanges.
/// </summary>
public sealed class ProcesarValeCommandHandler
    : IRequestHandler<ProcesarValeCommand, ValeResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IArticuloStockMovementService _stock;
    private readonly ICurrentUserService _currentUser;

    public ProcesarValeCommandHandler(
        IUnitOfWork uow,
        IArticuloStockMovementService stock,
        ICurrentUserService currentUser)
    {
        _uow = uow;
        _stock = stock;
        _currentUser = currentUser;
    }

    public async Task<ValeResponse> Handle(
        ProcesarValeCommand request,
        CancellationToken ct)
    {
        var planta = ValeRules.Normalize(request.PlantaCode);
        var code = ValeRules.Normalize(request.Code);

        var vale = await ValeReader.LoadForUpdateAsync(_uow, planta, code, ct);

        if (vale.Estado is not (EstadoVale.Aprobado or EstadoVale.EntregaParcial))
        {
            throw new InvalidOperationException(
                $"El Vale de Salida se encuentra {vale.Estado}; solo se procesa desde Aprobado o Entrega Parcial.");
        }

        if (ValeRules.EsTransferencia(vale))
        {
            throw new InvalidOperationException(
                "Un vale de tipo Transferencia no puede ser procesado por el almacén origen.");
        }

        await ValeRules.EnsurePeriodOpenAsync(
            _uow,
            planta,
            vale.FechaEntrega,
            "La Fecha de Entrega no puede ser menor o igual al cierre del período.",
            ct);

        // Solo las líneas pendientes: no se vuelve a descontar lo ya procesado.
        var pendientes = vale.Detalles
            .Where(d => d.Estado == EstadoValeDetalle.Pendiente)
            .ToList();

        if (pendientes.Count == 0)
        {
            throw new InvalidOperationException(
                "El Vale de Salida no tiene líneas pendientes por procesar.");
        }

        var seleccion = pendientes;

        if (request.Articulos is { Count: > 0 })
        {
            var solicitados = request.Articulos
                .Select(ValeRules.Normalize)
                .Distinct()
                .ToList();

            var noPendientes = solicitados
                .Where(a => pendientes.All(p => p.ArticuloCode != a))
                .ToList();

            if (noPendientes.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Los artículos {string.Join(", ", noPendientes)} no son líneas pendientes del vale.");
            }

            seleccion = pendientes
                .Where(p => solicitados.Contains(p.ArticuloCode))
                .ToList();
        }

        await EnsureStockSuficienteAsync(planta, seleccion, ct);

        foreach (var linea in seleccion.OrderBy(l => l.ItemNumber))
        {
            // Kardex: salida del vale al costo promedio vigente (el cierre la revaloriza si corresponde).
            await _stock.ApplyAsync(
                planta,
                linea.ArticuloCode,
                linea.Cantidad,
                0m,
                0m,
                0m,
                increment: false,
                recalculateAverage: false,
                context: new StockMovementContext(
                    OrigenMovimientoArticulo.ValeSalida, vale.Code, linea.ItemNumber, vale.FechaEntrega),
                cancellationToken: ct);

            linea.Estado = EstadoValeDetalle.Procesado;

            await ValeOrdenTrabajo.ConsumeAsync(_uow, vale, linea, ct);
        }

        var quedanPendientes = vale.Detalles
            .Any(d => d.Estado == EstadoValeDetalle.Pendiente);

        vale.Estado = quedanPendientes
            ? EstadoVale.EntregaParcial
            : EstadoVale.Procesado;

        vale.ProcesadoPor = _currentUser.FullName;
        vale.FechaProcesado = DateTime.Now;

        if (!quedanPendientes)
        {
            vale.Preparado = false;
        }

        await ValeReader.SaveAsync(_uow, ct);

        return await ValeReader.GetAsync(_uow, planta, code, ct);
    }

    /// <summary>
    /// Logi_sp_Consulta_Stock_VS: lista los artículos que dejarían saldo negativo
    /// (ítem, artículo, cantidad, stock y saldo) antes de tocar nada.
    /// </summary>
    private async Task EnsureStockSuficienteAsync(
        string planta,
        List<ValeDetalle> lineas,
        CancellationToken ct)
    {
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
            $"Existen artículos que al procesar el vale dejarían saldo negativo: {detalle}.");
    }
}
