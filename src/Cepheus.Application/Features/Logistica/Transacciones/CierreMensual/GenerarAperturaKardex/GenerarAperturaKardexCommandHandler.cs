using System.Data;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.GenerarAperturaKardex;

/// <summary>
/// Apertura del Kardex. Para cada artículo con stock: cantidad de apertura = stock actual − movimientos que el Kardex
/// ya tiene registrados (así no se duplica lo que se movió después de desplegar el Kardex y antes de ejecutar la
/// apertura). Es idempotente: no genera apertura a un artículo que ya la tiene. Usa el mismo bloqueo por planta que el
/// cierre, para que no corran a la vez. Es código de aplicación (sin script SQL), independiente del motor de base de datos.
/// </summary>
public sealed class GenerarAperturaKardexCommandHandler
    : IRequestHandler<GenerarAperturaKardexCommand, AperturaKardexResponse>
{
    private const decimal Tolerancia = 0.005m;
    private const int MaxCodigosInformados = 50;

    private readonly IUnitOfWork _uow;
    private readonly IArticuloStockMovementService _stock;

    public GenerarAperturaKardexCommandHandler(IUnitOfWork uow, IArticuloStockMovementService stock)
    {
        _uow = uow;
        _stock = stock;
    }

    public async Task<AperturaKardexResponse> Handle(GenerarAperturaKardexCommand request, CancellationToken ct)
    {
        var planta = request.PlantaCode.Trim().ToUpperInvariant();

        var existe = await _uow.Comunes.Plantas.Query().AsNoTracking().AnyAsync(p => p.Code == planta, ct);
        if (!existe)
            throw new KeyNotFoundException($"La planta {planta} no existe.");

        await CierreMensualLock.EnsureAsync(_uow, planta, ct);

        return await _uow.ExecuteInTransactionAsync(
            token => EjecutarAsync(planta, request.FechaApertura, token),
            IsolationLevel.ReadCommitted,
            ct);
    }

    private async Task<AperturaKardexResponse> EjecutarAsync(string planta, DateTime? fechaSolicitada, CancellationToken ct)
    {
        await CierreMensualLock.AcquireAsync(_uow, planta, ct);

        var movimientos = _uow.Logistica.Transacciones.MovimientosArticulo.Query().AsNoTracking()
            .Where(m => m.PlantaCode == planta);

        DateTime fecha;
        if (fechaSolicitada.HasValue)
        {
            fecha = fechaSolicitada.Value.Date;
        }
        else
        {
            var primera = await movimientos
                .OrderBy(m => m.FechaMovimiento)
                .Select(m => (DateTime?)m.FechaMovimiento)
                .FirstOrDefaultAsync(ct);

            fecha = (primera ?? DateTime.Today).Date;
        }

        var periodo = ControlCierreRules.PeriodOf(fecha);
        await ControlCierreRules.EnsurePeriodOpenAsync(
            _uow, planta, fecha,
            $"El período {periodo} ya está cerrado: no se puede generar la apertura con fecha {fecha:dd/MM/yyyy}.",
            ct);

        var kardex = (await movimientos
                .GroupBy(m => m.ArticuloCode)
                .Select(g => new
                {
                    Articulo = g.Key,
                    Neto = g.Sum(m => m.Tipo == TipoMovimientoArticulo.Ingreso ? m.Cantidad : -m.Cantidad),
                    TieneApertura = g.Any(m => m.Origen == OrigenMovimientoArticulo.Apertura)
                })
                .ToListAsync(ct))
            .ToDictionary(x => x.Articulo);

        var stocks = await _uow.Logistica.Maestros.StockArticulos.Query().AsNoTracking()
            .Where(s => s.PlantaCode == planta && s.Quantity > 0)
            .ToListAsync(ct);

        var conApertura = 0;
        var yaTenian = 0;
        var sinDiferencia = 0;
        var inconsistentes = new List<string>();

        foreach (var stock in stocks)
        {
            kardex.TryGetValue(stock.ArticuloCode, out var info);

            if (info?.TieneApertura == true)
            {
                yaTenian++;
                continue;
            }

            var cantidad = stock.Quantity - (info?.Neto ?? 0m);

            if (cantidad > Tolerancia)
            {
                await _stock.RecordOpeningAsync(stock, cantidad, fecha, ct);
                conApertura++;
            }
            else if (cantidad < -Tolerancia)
            {
                inconsistentes.Add(stock.ArticuloCode);
            }
            else
            {
                sinDiferencia++;
            }
        }

        await _uow.SaveChangesAsync(ct);

        return new AperturaKardexResponse(
            planta, fecha, conApertura, yaTenian, sinDiferencia,
            inconsistentes.Count,
            inconsistentes.Take(MaxCodigosInformados).ToList());
    }
}
