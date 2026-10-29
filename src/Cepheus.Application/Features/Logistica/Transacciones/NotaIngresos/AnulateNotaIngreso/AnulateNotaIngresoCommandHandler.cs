using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.StockArticulos.Common;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.AnulateNotaIngreso;

/// <summary>
/// Un solo SaveChanges: nota, líneas, stock y OC quedan anulados/revertidos juntos o ninguno.
/// </summary>
public sealed class AnulateNotaIngresoCommandHandler
    : IRequestHandler<AnulateNotaIngresoCommand, NotaIngresoResponse>
{
    private readonly IUnitOfWork _uow;
    private readonly IArticuloStockMovementService _stock;

    public AnulateNotaIngresoCommandHandler(IUnitOfWork uow, IArticuloStockMovementService stock)
    {
        _uow = uow;
        _stock = stock;
    }

    public async Task<NotaIngresoResponse> Handle(AnulateNotaIngresoCommand request, CancellationToken ct)
    {
        var planta = NotaIngresoRules.Normalize(request.PlantaCode);
        var code = NotaIngresoRules.Normalize(request.Code);

        var note = await _uow.Logistica.Transacciones.NotaIngresos.Query()
            .Include(x => x.Detalles)
            .Include(x => x.ComprobantePago)
            .Include(x => x.MotivoDevolucion)
            .FirstOrDefaultAsync(x => x.PlantaCode == planta && x.Code == code, ct)
            ?? throw new KeyNotFoundException($"La Nota de Ingreso {planta}/{code} no existe.");

        if (note.Estado == EstadoNotaIngreso.Anulado)
            throw new InvalidOperationException("La Nota de Ingreso ya está anulada.");

        if (!string.IsNullOrWhiteSpace(note.AsientoContable))
            throw new InvalidOperationException(
                "La Nota de Ingreso tiene asiento contable. Imposible su anulación.");

        if (!string.IsNullOrWhiteSpace(note.CodigoVale))
            throw new InvalidOperationException(
                "La Nota de Ingreso tiene un Vale de Salida asociado. Imposible su anulación.");

        if (note.Condicion != CondicionNotaIngreso.OrdenCompra)
            throw new InvalidOperationException("La anulación de notas con guías anexadas aún no está disponible.");
        if (note.Origen is not (OrigenNotaIngreso.Compra or OrigenNotaIngreso.Importacion or OrigenNotaIngreso.Transferencia))
            throw new InvalidOperationException("El origen de la Nota de Ingreso no admite anulación.");

        await NotaIngresoRules.EnsurePeriodOpenAsync(
            _uow, planta, note.FechaRecepcion,
            "La Nota de Ingreso está procesada y su período está cerrado. Imposible su anulación.", ct);

        var lineas = note.Detalles.Where(d => d.Estado == EstadoNotaIngresoDetalle.Procesado).ToList();

        if (note.Origen == OrigenNotaIngreso.Transferencia)
            await AnularTransferenciaAsync(note, lineas, planta, ct);
        else if (note.Origen == OrigenNotaIngreso.Importacion)
            await AnularImportacionAsync(note, lineas, planta, ct);
        else if (note.ComprobantePago.Code == NotaIngresoRules.ComprobanteNotaCredito)
            await AnularNotaCreditoAsync(note, lineas, planta, ct);
        else
            await AnularRecepcionAsync(note, lineas, planta, ct);

        foreach (var linea in lineas)
            linea.Estado = EstadoNotaIngresoDetalle.Anulado;
        note.Estado = EstadoNotaIngreso.Anulado;

        await _uow.SaveChangesAsync(ct);

        return await NotaIngresoReader.GetAsync(_uow, planta, code, ct);
    }

    /// <summary>
    /// Nota de Ingreso de una transferencia entre almacenes (Logi_sp_Anula_NotaIngreso, origen 'T'):
    ///   - resta del DESTINO lo que ingresó (el servicio central rechaza si el stock ya no alcanza: es el
    ///     "dejan saldos negativos" de la ventana del legacy);
    ///   - devuelve al ORIGEN lo que había salido (deshace la salida del vale al costo con que salió);
    ///   - las líneas y la cabecera del vale vuelven a Pendiente, para poder recibirlo de nuevo.
    /// Ambos movimientos se registran como reversión de los originales en el Kardex y respetan el período cerrado de
    /// cada planta (con la fecha de recepción de la nota). El costo promedio del destino no se revierte, igual que en
    /// el legacy. Los asientos contables quedan fuera de esta fase.
    /// </summary>
    private async Task AnularTransferenciaAsync(
        NotaIngreso note,
        List<NotaIngresoDetalle> lineas,
        string destino,
        CancellationToken ct)
    {
        var origen = note.PlantaOrigenCode
            ?? throw new InvalidOperationException("La Nota de Ingreso de transferencia no tiene planta origen.");
        var valeCode = note.ValeTransferenciaCode
            ?? throw new InvalidOperationException("La Nota de Ingreso de transferencia no tiene vale asociado.");

        var vale = await _uow.Logistica.Transacciones.Vales.Query()
            .Include(v => v.Detalles)
            .FirstOrDefaultAsync(v => v.PlantaCode == origen && v.Code == valeCode, ct)
            ?? throw new KeyNotFoundException($"El vale {origen}/{valeCode} de la transferencia no existe.");

        if (vale.Estado != EstadoVale.Procesado)
            throw new InvalidOperationException(
                $"El vale {origen}/{valeCode} se encuentra {vale.Estado}: no se puede anular la transferencia.");

        foreach (var linea in lineas)
        {
            // Destino: resta lo ingresado.
            await _stock.ApplyAsync(
                destino, linea.ArticuloCode, linea.Cantidad, 0m, 0m, 0m,
                increment: false, recalculateAverage: false,
                context: Reversion(OrigenMovimientoArticulo.TransferenciaIngreso, note, linea),
                cancellationToken: ct);

            // Origen: devuelve lo que salió (la salida se registró con el código del vale y el mismo ítem).
            await _stock.ApplyAsync(
                origen, linea.ArticuloCode, linea.Cantidad, 0m, 0m, 0m,
                increment: true, recalculateAverage: false,
                context: new StockMovementContext(
                    OrigenMovimientoArticulo.ValeSalida, valeCode, linea.ItemNumber, note.FechaRecepcion,
                    EsReversion: true),
                cancellationToken: ct);

            var lineaVale = vale.Detalles.FirstOrDefault(d =>
                d.ItemNumber == linea.ItemNumber && d.Estado == EstadoValeDetalle.Procesado);

            if (lineaVale is not null)
                lineaVale.Estado = EstadoValeDetalle.Pendiente;
        }

        vale.Estado = EstadoVale.Pendiente;
        vale.ProcesadoPor = null;
        vale.FechaProcesado = null;
    }

    /// <summary>
    /// Nota de Ingreso de Importación: resta el stock ingresado (el servicio central rechaza si el stock ya
    /// no alcanza, que es el caso de "el stock se movió") y devuelve a Pendiente las líneas de la importación
    /// que originaron la nota y la importación misma, para poder regenerarla.
    /// </summary>
    private async Task AnularImportacionAsync(
        NotaIngreso note,
        List<NotaIngresoDetalle> lineas,
        string planta,
        CancellationToken ct)
    {
        var importacionCode = note.ImportacionCode
            ?? throw new InvalidOperationException("La Nota de Ingreso no tiene Importación asociada.");

        var importacion = await _uow.Logistica.Transacciones.Importaciones.Query()
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync(x => x.PlantaCode == planta && x.Code == importacionCode, ct)
            ?? throw new KeyNotFoundException($"La Importación {importacionCode} no existe en la planta {planta}.");

        if (importacion.Estado == EstadoImportacion.Anulado)
            throw new InvalidOperationException("La Importación asociada está anulada.");

        foreach (var linea in lineas)
        {
            await _stock.ApplyAsync(
                planta, linea.ArticuloCode, linea.Cantidad, 0m, 0m, 0m,
                increment: false, recalculateAverage: false,
                context: Reversion(OrigenMovimientoArticulo.Importacion, note, linea),
                cancellationToken: ct);

            var detalle = importacion.Detalles.FirstOrDefault(d =>
                d.ProveedorCode == note.ProveedorCode
                && d.ArticuloCode == linea.ArticuloCode
                && d.Estado == EstadoImportacion.Procesado);

            if (detalle is not null)
                detalle.Estado = EstadoImportacion.Pendiente;
        }

        importacion.Estado = EstadoImportacion.Pendiente;
    }

    /// <summary>
    /// Nota de Crédito: solo la devolución (motivo 02) descontó stock al registrarse, así que solo ella
    /// lo repone. El ajuste (motivo 01) no toca stock. No afecta a la OC.
    /// </summary>
    private async Task AnularNotaCreditoAsync(
        NotaIngreso note,
        List<NotaIngresoDetalle> lineas,
        string planta,
        CancellationToken ct)
    {
        if (note.MotivoDevolucion?.Code != NotaIngresoRules.MotivoDevolucion)
            return;

        foreach (var linea in lineas)
        {
            await _stock.ApplyAsync(
                planta, linea.ArticuloCode, linea.Cantidad, 0m, 0m, 0m,
                increment: true, recalculateAverage: false,
                context: Reversion(OrigenMovimientoArticulo.NotaCredito, note, linea),
                cancellationToken: ct);
        }
    }

    /// <summary>
    /// Recepción contra OC: resta el stock ingresado (sin permitir stock negativo, lo valida el servicio
    /// central), revierte lo entregado en la OC por línea y pedido, y recalcula el estado de la OC.
    /// </summary>
    private async Task AnularRecepcionAsync(
        NotaIngreso note,
        List<NotaIngresoDetalle> lineas,
        string planta,
        CancellationToken ct)
    {
        var ocCode = note.OrdenCompraCode
            ?? throw new InvalidOperationException("La Nota de Ingreso no tiene Orden de Compra asociada.");

        var oc = await _uow.Logistica.Transacciones.OrdenesCompra.Query()
            .Include(x => x.Detalles)
                .ThenInclude(d => d.Origenes)
            .FirstOrDefaultAsync(x => x.PlantaCode == planta && x.Code == ocCode, ct)
            ?? throw new KeyNotFoundException($"La Orden de Compra {ocCode} no existe en la planta {planta}.");

        foreach (var linea in lineas)
        {
            // Si el stock no alcanza (ya se consumió), el servicio lanza InvalidOperationException (409).
            await _stock.ApplyAsync(
                planta, linea.ArticuloCode, linea.Cantidad, 0m, 0m, 0m,
                increment: false, recalculateAverage: false,
                context: Reversion(OrigenMovimientoArticulo.NotaIngreso, note, linea),
                cancellationToken: ct);

            var ocDetalle = oc.Detalles.FirstOrDefault(d => d.ArticuloCode == linea.ArticuloCode);
            if (ocDetalle is not null)
                NotaIngresoOrdenCompraUpdater.RevertDelivery(ocDetalle, linea.PedidoCode, linea.Cantidad);
        }

        NotaIngresoOrdenCompraUpdater.RecalculateEstado(oc);
    }

    /// <summary>
    /// Contexto de Kardex de una anulación: deshace el movimiento original de la línea (mismo documento,
    /// ítem y origen) con la fecha de recepción de la nota, que es la que define su período.
    /// </summary>
    private static StockMovementContext Reversion(
        OrigenMovimientoArticulo origen, NotaIngreso note, NotaIngresoDetalle linea) =>
        new(origen, note.Code, linea.ItemNumber, note.FechaRecepcion, EsReversion: true);
}
