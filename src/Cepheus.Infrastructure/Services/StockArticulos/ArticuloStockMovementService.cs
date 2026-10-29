using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Logistica.Transacciones;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Infrastructure.Services.StockArticulos;

/// <summary>
/// Implementación centralizada del movimiento de stock (único punto que modifica ArticuloStock;
/// lo usan Nota de Ingreso, Importaciones, Orden de Servicio, Vales, Ajustes y cualquier proceso futuro).
/// Traduce la regla de Logi_sp_Actualiza_Stock_Planta_Articulo:
/// - entrada con recálculo: promedio ponderado considerando el descuento (en porcentaje);
/// - entrada sin recálculo: solo incrementa la cantidad;
/// - salida: nunca permite stock negativo (el SP no lo valida; lo validaba el PowerBuilder antes);
/// - si no existe registro, una entrada lo crea con Precio promedio = costo unitario.
///
/// Además:
/// - rechaza el movimiento si el período de su fecha ya está cerrado (ControlCierre);
/// - deja una fila en el Kardex (MovimientoArticulo) por cada movimiento, en la misma unidad de trabajo.
///
/// No llama SaveChanges: el caso de uso dueño del movimiento confirma todo en una sola transacción.
/// La concurrencia entre usuarios la resuelve el RowVersion de ArticuloStock: si dos procesos tocan el
/// mismo artículo, el segundo falla con DbUpdateConcurrencyException y no queda ningún movimiento a medias.
///
/// Si el mismo artículo se mueve varias veces dentro de una misma unidad de trabajo y el stock aún no
/// existe en BD, el registro recién creado se reutiliza (no se intenta insertar dos veces).
/// </summary>
public sealed class ArticuloStockMovementService : IArticuloStockMovementService
{
    // Costos con 6 decimales: AverageCost/UnitCost son decimal(18,6) y el cierre legacy trabaja a 6.
    private const int CostDecimals = 6;

    private readonly IUnitOfWork _uow;

    // Stocks creados en esta unidad de trabajo y todavía sin guardar (el servicio es scoped).
    private readonly Dictionary<(string Planta, string Articulo), ArticuloStock> _pendingNew = new();

    // Períodos ya verificados como abiertos en esta unidad de trabajo (evita repetir la consulta por línea).
    private readonly HashSet<(string Planta, string Periodo)> _openPeriods = new();

    public ArticuloStockMovementService(IUnitOfWork uow) => _uow = uow;

    public async Task<StockMovementResult> ApplyAsync(
        string plantaCode,
        string articuloCode,
        decimal quantity,
        decimal unitCost,
        decimal unitCostUsd,
        decimal discountPercent,
        bool increment,
        bool recalculateAverage,
        StockMovementContext context,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "La cantidad debe ser mayor que cero.");

        ArgumentNullException.ThrowIfNull(context);

        plantaCode = plantaCode.Trim().ToUpperInvariant();
        articuloCode = articuloCode.Trim().ToUpperInvariant();

        var fecha = context.FechaMovimiento.Date;
        await EnsurePeriodOpenAsync(plantaCode, fecha, cancellationToken);

        if (!_pendingNew.TryGetValue((plantaCode, articuloCode), out var stock))
        {
            stock = await _uow.Logistica.Maestros.StockArticulos.Query()
                .FirstOrDefaultAsync(
                    x => x.PlantaCode == plantaCode && x.ArticuloCode == articuloCode,
                    cancellationToken);
        }

        // Costo (soles netos y dólares) que quedará anotado en el Kardex para este movimiento.
        decimal movementCost;
        decimal movementCostUsd;

        if (stock is null)
        {
            if (!increment)
                throw new InvalidOperationException(
                    $"No existe stock para {articuloCode} en planta {plantaCode}.");

            stock = new ArticuloStock
            {
                PlantaCode = plantaCode,
                ArticuloCode = articuloCode,
                Quantity = quantity,
                UnitCost = unitCost,
                UnitCostUsd = unitCostUsd,
                AverageCost = unitCost
            };

            await _uow.Logistica.Maestros.StockArticulos.AddAsync(stock, cancellationToken);
            _pendingNew[(plantaCode, articuloCode)] = stock;

            movementCost = recalculateAverage
                ? Discounted(unitCost, discountPercent)
                : context.CostoReferencia ?? unitCost;
            movementCostUsd = unitCostUsd;
        }
        else if (!increment)
        {
            if (stock.Quantity < quantity)
            {
                throw new InvalidOperationException(
                    $"Stock insuficiente de {articuloCode} en {plantaCode}. " +
                    $"Disponible: {stock.Quantity}, solicitado: {quantity}.");
            }

            movementCost = context.CostoReferencia ?? CurrentAverage(stock);
            movementCostUsd = stock.UnitCostUsd;

            stock.Quantity -= quantity;
        }
        else if (!recalculateAverage)
        {
            movementCost = context.CostoReferencia ?? CurrentAverage(stock);
            movementCostUsd = stock.UnitCostUsd;

            stock.Quantity += quantity;
        }
        else
        {
            // SP: Precio_Dsto = Costo_Uni - Costo_Uni * (Descuento/100);
            //     Precio_Pro  = (Stock_Actual * Precio_Pro + Stock * Precio_Dsto) / (Stock + Stock_Actual)
            var currentAverage = CurrentAverage(stock);
            var discountedCost = Discounted(unitCost, discountPercent);
            var newQuantity = stock.Quantity + quantity;

            stock.AverageCost = newQuantity == 0
                ? discountedCost
                : Math.Round(
                    ((stock.Quantity * currentAverage) + (quantity * discountedCost)) / newQuantity,
                    CostDecimals, MidpointRounding.AwayFromZero);

            stock.Quantity = newQuantity;
            stock.UnitCost = unitCost;
            stock.UnitCostUsd = unitCostUsd;

            movementCost = discountedCost;
            movementCostUsd = unitCostUsd;
        }

        var tipo = increment ? TipoMovimientoArticulo.Ingreso : TipoMovimientoArticulo.Salida;

        var movimiento = new MovimientoArticulo
        {
            PlantaCode = plantaCode,
            ArticuloCode = articuloCode,
            FechaMovimiento = fecha,
            PeriodCode = ControlCierreRules.PeriodOf(fecha),
            Tipo = tipo,
            Origen = context.Origen,
            DocumentoCode = context.DocumentoCode!, // null => falla al guardar (ver StockMovementContext)
            ItemNumber = context.ItemNumber,
            Cantidad = quantity,
            EsReversion = context.EsReversion,
            StockPosterior = stock.Quantity,
            CostoPromedioPosterior = CurrentAverage(stock)
        };

        if (context.EsReversion)
        {
            var original = await FindOriginalAsync(plantaCode, articuloCode, tipo, context, cancellationToken);

            if (original is not null)
            {
                movimiento.ReversaDeMovimientoId = original.Id;

                // Una reversión sale/entra al mismo costo con el que se registró el movimiento original.
                if (context.CostoReferencia is null)
                {
                    movementCost = original.CostoUnitario;
                    movementCostUsd = original.CostoUnitarioUsd;
                }
            }
        }

        movimiento.CostoUnitario = movementCost;
        movimiento.CostoUnitarioUsd = movementCostUsd;

        await _uow.Logistica.Transacciones.MovimientosArticulo.AddAsync(movimiento, cancellationToken);

        return new StockMovementResult(stock, movimiento);
    }

    public Task<MovimientoArticulo?> RecordOpeningAsync(
        ArticuloStock stock,
        DateTime fecha,
        CancellationToken cancellationToken = default)
        => RecordOpeningAsync(stock, stock.Quantity, fecha, cancellationToken);

    public async Task<MovimientoArticulo?> RecordOpeningAsync(
        ArticuloStock stock,
        decimal cantidad,
        DateTime fecha,
        CancellationToken cancellationToken = default)
    {
        if (cantidad <= 0)
            return null;

        var plantaCode = stock.PlantaCode.Trim().ToUpperInvariant();
        var articuloCode = stock.ArticuloCode.Trim().ToUpperInvariant();
        fecha = fecha.Date;

        await EnsurePeriodOpenAsync(plantaCode, fecha, cancellationToken);

        var movimiento = new MovimientoArticulo
        {
            PlantaCode = plantaCode,
            ArticuloCode = articuloCode,
            FechaMovimiento = fecha,
            PeriodCode = ControlCierreRules.PeriodOf(fecha),
            Tipo = TipoMovimientoArticulo.Ingreso,
            Origen = OrigenMovimientoArticulo.Apertura,
            DocumentoCode = "APERTURA",
            ItemNumber = null,
            Cantidad = cantidad,
            CostoUnitario = CurrentAverage(stock),
            CostoUnitarioUsd = stock.UnitCostUsd,
            StockPosterior = stock.Quantity,
            CostoPromedioPosterior = CurrentAverage(stock),
            EsReversion = false
        };

        await _uow.Logistica.Transacciones.MovimientosArticulo.AddAsync(movimiento, cancellationToken);
        return movimiento;
    }

    private async Task EnsurePeriodOpenAsync(string plantaCode, DateTime fecha, CancellationToken ct)
    {
        var period = ControlCierreRules.PeriodOf(fecha);

        if (_openPeriods.Contains((plantaCode, period)))
            return;

        await ControlCierreRules.EnsurePeriodOpenAsync(
            _uow, plantaCode, fecha,
            $"El período {period} de la planta {plantaCode} está cerrado: " +
            $"no se puede mover el stock con fecha {fecha:dd/MM/yyyy}.",
            ct);

        _openPeriods.Add((plantaCode, period));
    }

    /// <summary>
    /// Movimiento original que esta reversión deshace: mismo documento, ítem y origen, de sentido contrario
    /// y que todavía no haya sido revertido. Null si el documento es anterior al Kardex (no hay original).
    /// </summary>
    private async Task<OriginalMovimiento?> FindOriginalAsync(
        string plantaCode,
        string articuloCode,
        TipoMovimientoArticulo tipoReversion,
        StockMovementContext context,
        CancellationToken ct)
    {
        var tipoOriginal = tipoReversion == TipoMovimientoArticulo.Ingreso
            ? TipoMovimientoArticulo.Salida
            : TipoMovimientoArticulo.Ingreso;

        var movimientos = _uow.Logistica.Transacciones.MovimientosArticulo.Query().AsNoTracking();

        return await movimientos
            .Where(m =>
                m.PlantaCode == plantaCode &&
                m.ArticuloCode == articuloCode &&
                m.Origen == context.Origen &&
                m.DocumentoCode == context.DocumentoCode &&
                m.ItemNumber == context.ItemNumber &&
                m.Tipo == tipoOriginal &&
                !m.EsReversion &&
                !movimientos.Any(r => r.ReversaDeMovimientoId == m.Id))
            .OrderByDescending(m => m.Id)
            .Select(m => new OriginalMovimiento(m.Id, m.CostoUnitario, m.CostoUnitarioUsd))
            .FirstOrDefaultAsync(ct);
    }

    private static decimal CurrentAverage(ArticuloStock stock) => stock.AverageCost ?? stock.UnitCost;

    private static decimal Discounted(decimal unitCost, decimal discountPercent) =>
        Math.Round(
            unitCost - (unitCost * (discountPercent / 100m)),
            CostDecimals, MidpointRounding.AwayFromZero);

    private sealed record OriginalMovimiento(long Id, decimal CostoUnitario, decimal CostoUnitarioUsd);
}
