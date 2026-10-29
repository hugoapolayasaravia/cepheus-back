using System.Data;
using System.Globalization;
using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Kardex.Common;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using Cepheus.Domain.Logistica.Maestros;
using Cepheus.Domain.Logistica.Transacciones;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.CierreMensual.ProcesarCierreMensual;

/// <summary>
/// Cierre mensual de artículos. Reemplaza los tres SP del legacy (Consumos, Mes_Cierre, AgregaTControl).
///
/// - UNA transacción para toda la planta: si algo falla no queda nada a medias (el legacy tenía el BEGIN TRAN
///   comentado). Una fila de bloqueo por planta (CierreMensualLock) evita dos cierres simultáneos. Antes de confirmar
///   se compara la huella de los movimientos del período: si alguien registró uno mientras se procesaba, se revierte
///   y se pide reintentar. No depende de triggers ni de funciones propias de un motor de base de datos.
/// - Solo cierra el período que sigue al último cerrado y solo si el período ya terminó.
/// - Saldos y costos se calculan en CierreMensualCalculator (mismas reglas que el Kardex).
/// - NO sobrescribe la cantidad del stock vivo (el legacy pisaba Stock_Plt_Art): la concilia contra saldo +
///   movimientos posteriores y la diferencia queda en ControlCierre.DifferenceAmount e incidencias. El costo
///   promedio vivo sí se alinea con el del cierre (como el legacy hacía con Precio_Pro).
/// - Las incidencias van a CierreIncidencias (por planta y período), no a una tabla global.
/// - Los precios de los documentos de salida se corrigen vía ICierreRevaluador y cada cambio queda auditado.
/// </summary>
public sealed class ProcesarCierreMensualCommandHandler
    : IRequestHandler<ProcesarCierreMensualCommand, CierreMensualResponse>
{
    private const decimal ToleranciaCantidad = 0.005m;
    private const decimal ToleranciaCosto = 0.0000005m;

    private readonly IUnitOfWork _uow;
    private readonly IReadOnlyList<ICierreRevaluador> _revaluadores;
    private readonly IReadOnlyList<IPropagadorRevaluacion> _propagadores;

    public ProcesarCierreMensualCommandHandler(
        IUnitOfWork uow,
        IEnumerable<ICierreRevaluador> revaluadores,
        IEnumerable<IPropagadorRevaluacion> propagadores)
    {
        _uow = uow;
        _revaluadores = revaluadores.ToList();
        _propagadores = propagadores.ToList();
    }

    public async Task<CierreMensualResponse> Handle(ProcesarCierreMensualCommand request, CancellationToken ct)
    {
        var planta = request.PlantaCode.Trim().ToUpperInvariant();

        var existe = await _uow.Comunes.Plantas.Query().AsNoTracking().AnyAsync(p => p.Code == planta, ct);
        if (!existe)
            throw new KeyNotFoundException($"La planta {planta} no existe.");

        await CierreMensualLock.EnsureAsync(_uow, planta, ct);

        try
        {
            return await _uow.ExecuteInTransactionAsync(
                token => EjecutarAsync(planta, request.PeriodoCode?.Trim(), token),
                IsolationLevel.ReadCommitted,
                ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new InvalidOperationException(
                "El stock o un documento cambió mientras se procesaba el cierre. Intente nuevamente.");
        }
    }

    private async Task<CierreMensualResponse> EjecutarAsync(string planta, string? periodoSolicitado, CancellationToken ct)
    {
        await CierreMensualLock.AcquireAsync(_uow, planta, ct);

        // ------------------------------------------------------------ 1. qué período se cierra
        var ultimo = await ControlCierreRules.GetLastClosedPeriodAsync(_uow, planta, ct);
        var movimientosDb = _uow.Logistica.Transacciones.MovimientosArticulo.Query().AsNoTracking();

        string periodo;
        if (ultimo is null)
        {
            if (string.IsNullOrWhiteSpace(periodoSolicitado))
                throw new InvalidOperationException(
                    "Es el primer cierre de la planta: indique el período de apertura (AAAAMM).");

            periodo = periodoSolicitado;

            var hayAnteriores = await movimientosDb.AnyAsync(
                m => m.PlantaCode == planta && string.Compare(m.PeriodCode, periodo) < 0, ct);

            if (hayAnteriores)
                throw new InvalidOperationException(
                    $"Hay movimientos en períodos anteriores a {periodo}. El primer cierre debe ser el período del primer movimiento.");
        }
        else
        {
            periodo = ControlCierreRules.NextPeriod(ultimo);

            if (periodoSolicitado is not null && periodoSolicitado != periodo)
                throw new InvalidOperationException(
                    $"El siguiente período a cerrar es {periodo} (último cerrado: {ultimo}).");
        }

        var inicio = DateTime.ParseExact(periodo + "01", "yyyyMMdd", CultureInfo.InvariantCulture);
        var fin = inicio.AddMonths(1).AddDays(-1);

        if (fin >= DateTime.Today)
            throw new InvalidOperationException(
                $"El período {periodo} aún no termina (finaliza el {fin:dd/MM/yyyy}): no se puede cerrar.");

        // ------------------------------------------------------------ 2. datos del período
        // Huella de los movimientos del período: se vuelve a leer antes de confirmar para detectar movimientos
        // registrados mientras se procesaba (reemplaza a la protección que daría un trigger).
        var huellaInicial = await LeerHuellaAsync(planta, periodo, ct);

        var filas = await movimientosDb
            .Where(m => m.PlantaCode == planta && m.PeriodCode == periodo)
            .Select(m => new
            {
                m.Id, m.ArticuloCode, m.FechaMovimiento, m.PeriodCode, m.Tipo, m.Origen, m.Cantidad,
                m.CostoUnitario, m.EsReversion, m.ReversaDeMovimientoId, m.DocumentoCode, m.ItemNumber
            })
            .ToListAsync(ct);

        if (ultimo is null && filas.Count == 0)
            throw new InvalidOperationException(
                $"No hay movimientos en el período {periodo}: verifique el período de apertura.");

        var saldosBase = ultimo is null
            ? new Dictionary<string, (decimal Cantidad, decimal Costo)>()
            : (await _uow.Logistica.Transacciones.SaldosMensualesArticulo.Query().AsNoTracking()
                .Where(s => s.PlantaCode == planta && s.PeriodCode == ultimo)
                .Select(s => new { s.ArticuloCode, s.Quantity, s.AverageCost })
                .ToListAsync(ct))
                .ToDictionary(s => s.ArticuloCode, s => (Cantidad: s.Quantity, Costo: s.AverageCost));

        // Notas de Crédito por ajuste de precio (motivo 01): no mueven stock, solo valorización.
        var notas = await _uow.Logistica.Transacciones.NotaIngresos.Query().AsNoTracking()
            .Where(n =>
                n.PlantaCode == planta &&
                n.ComprobantePagoCode == NotaIngresoRules.ComprobanteNotaCredito &&
                n.MotivoDevolucionCode == NotaIngresoRules.MotivoAjuste &&
                n.Estado == EstadoNotaIngreso.Procesado &&
                n.FechaRecepcion >= inicio && n.FechaRecepcion < inicio.AddMonths(1))
            .Select(n => new
            {
                n.Code, n.FechaRecepcion, n.MonedaCode, n.TipoCambio,
                Lineas = n.Detalles
                    .Where(d => d.Estado == EstadoNotaIngresoDetalle.Procesado)
                    .Select(d => new { d.ArticuloCode, d.Cantidad, d.Precio, d.Descuento })
                    .ToList()
            })
            .ToListAsync(ct);

        var ajustesPorArticulo = notas
            .SelectMany(n => n.Lineas.Select(l =>
            {
                var soles = NotaIngresoRules.ConvertPrices(n.MonedaCode, l.Precio, n.TipoCambio).Soles;
                var neto = Math.Round(soles - (soles * (l.Descuento / 100m)), 6, MidpointRounding.AwayFromZero);
                return (Articulo: l.ArticuloCode, Nota: new AjustePrecioNota(n.Code, n.FechaRecepcion.Date, l.Cantidad, neto));
            }))
            .GroupBy(x => x.Articulo)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<AjustePrecioNota>)g.Select(x => x.Nota).ToList());

        var movimientosPorArticulo = filas
            .GroupBy(f => f.ArticuloCode)
            .ToDictionary(g => g.Key, g => g.ToList());

        var infoPorMovimiento = filas.ToDictionary(f => f.Id);

        // ------------------------------------------------------------ 3. cálculo por artículo
        var articulos = saldosBase.Where(kv => kv.Value.Cantidad != 0).Select(kv => kv.Key)
            .Union(movimientosPorArticulo.Keys)
            .Union(ajustesPorArticulo.Keys)
            .Distinct()
            .OrderBy(a => a, StringComparer.Ordinal)
            .ToList();

        var resultados = new List<CierreArticuloResultado>(articulos.Count);

        foreach (var articulo in articulos)
        {
            saldosBase.TryGetValue(articulo, out var baseSaldo);
            movimientosPorArticulo.TryGetValue(articulo, out var movs);
            ajustesPorArticulo.TryGetValue(articulo, out var ajustes);

            var entradas = new List<KardexEntrada>();
            if (movs is not null)
            {
                entradas.AddRange(movs.Select(f => new KardexEntrada(
                    f.Id, f.FechaMovimiento, f.PeriodCode, f.Tipo, f.Origen,
                    f.Cantidad, f.CostoUnitario, f.EsReversion, f.ReversaDeMovimientoId)));
            }

            resultados.Add(CierreMensualCalculator.Calcular(new CierreArticuloEntrada(
                articulo, baseSaldo.Cantidad, baseSaldo.Costo, entradas,
                ajustes ?? Array.Empty<AjustePrecioNota>())));
        }

        // ------------------------------------------------------------ 4. saldos del período
        var saldosNuevos = new Dictionary<string, decimal>();
        decimal importeTotal = 0;
        var conSaldo = 0;

        foreach (var r in resultados)
        {
            if (!r.TuvoMovimientos && r.Cantidad == 0)
                continue;

            await _uow.Logistica.Transacciones.SaldosMensualesArticulo.AddAsync(new SaldoMensualArticulo
            {
                PlantaCode = planta,
                PeriodCode = periodo,
                ArticuloCode = r.ArticuloCode,
                Quantity = r.Cantidad,
                AverageCost = r.CostoPromedio,
                IncomingQuantity = Math.Round(r.CantidadIngresos, 2, MidpointRounding.AwayFromZero),
                OutgoingQuantity = Math.Round(r.CantidadSalidas, 2, MidpointRounding.AwayFromZero)
            }, ct);

            saldosNuevos[r.ArticuloCode] = r.Cantidad;
            importeTotal += KardexValorizador.Importe(r.Cantidad, r.CostoPromedio);
            conSaldo++;
        }

        // ------------------------------------------------------------ 5. revalorización de documentos de salida
        var porRevaluar = resultados
            .SelectMany(r => r.Salidas.Select(s =>
            {
                var info = infoPorMovimiento[s.MovimientoId];
                return new SalidaValorizada(s.Origen, info.DocumentoCode, info.ItemNumber, r.ArticuloCode, s.Cantidad, s.PrecioNuevo);
            }))
            .ToList();

        var documentosRevalorizados = 0;
        var todasAplicadas = new List<RevaluacionAplicada>();

        async Task AuditarAsync(IReadOnlyList<RevaluacionAplicada> aplicadas)
        {
            foreach (var a in aplicadas)
            {
                await _uow.Logistica.Transacciones.CierreRevaluaciones.AddAsync(new CierreRevaluacion
                {
                    PlantaCode = planta,
                    PeriodCode = periodo,
                    Origen = a.Origen,
                    DocumentoCode = a.DocumentoCode,
                    ItemNumber = a.ItemNumber,
                    ArticuloCode = a.ArticuloCode,
                    Cantidad = a.Cantidad,
                    PrecioAnterior = a.PrecioAnterior,
                    PrecioNuevo = a.PrecioNuevo,
                    TotalAnterior = a.TotalAnterior,
                    TotalNuevo = a.TotalNuevo
                }, ct);
            }

            documentosRevalorizados += aplicadas.Select(a => (a.Origen, a.DocumentoCode)).Distinct().Count();
        }

        foreach (var revaluador in _revaluadores)
        {
            var propias = porRevaluar.Where(s => revaluador.Origenes.Contains(s.Origen)).ToList();
            if (propias.Count == 0)
                continue;

            var aplicadas = await revaluador.AplicarAsync(planta, periodo, propias, ct);
            todasAplicadas.AddRange(aplicadas);
            await AuditarAsync(aplicadas);
        }

        // Segunda pasada: lo corregido en un documento que debe llevarse a otro (costo de la transferencia al ingreso
        // del almacén destino). Sus incidencias se registran más abajo junto con las demás.
        var incidenciasPropagacion = new List<PropagacionIncidencia>();
        foreach (var propagador in _propagadores)
        {
            var propagacion = await propagador.PropagarAsync(planta, periodo, todasAplicadas, ct);
            await AuditarAsync(propagacion.Aplicadas);
            incidenciasPropagacion.AddRange(propagacion.Incidencias);
        }

        // ------------------------------------------------------------ 6. incidencias y conciliación con el stock
        var incidencias = 0;

        foreach (var r in resultados)
        {
            foreach (var i in r.Incidencias)
            {
                await _uow.Logistica.Transacciones.CierreIncidencias.AddAsync(new CierreIncidencia
                {
                    PlantaCode = planta,
                    PeriodCode = periodo,
                    ArticuloCode = r.ArticuloCode,
                    Tipo = i.Tipo,
                    DocumentoCode = i.DocumentoCode,
                    Mensaje = Truncar(i.Mensaje)
                }, ct);
                incidencias++;
            }
        }

        foreach (var i in incidenciasPropagacion)
        {
            await _uow.Logistica.Transacciones.CierreIncidencias.AddAsync(new CierreIncidencia
            {
                PlantaCode = planta,
                PeriodCode = periodo,
                ArticuloCode = i.ArticuloCode,
                Tipo = i.Tipo,
                DocumentoCode = i.DocumentoCode,
                Mensaje = Truncar(i.Mensaje)
            }, ct);
            incidencias++;
        }

        // Movimientos de períodos posteriores: sirven para conciliar el stock y para alinear el costo vivo.
        var filasPosteriores = await movimientosDb
            .Where(m => m.PlantaCode == planta && string.Compare(m.PeriodCode, periodo) > 0)
            .Select(m => new
            {
                m.Id, m.ArticuloCode, m.FechaMovimiento, m.PeriodCode, m.Tipo, m.Origen, m.Cantidad,
                m.CostoUnitario, m.EsReversion, m.ReversaDeMovimientoId
            })
            .ToListAsync(ct);

        var posterioresPorArticulo = filasPosteriores
            .GroupBy(f => f.ArticuloCode)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => new KardexEntrada(
                    f.Id, f.FechaMovimiento, f.PeriodCode, f.Tipo, f.Origen,
                    f.Cantidad, f.CostoUnitario, f.EsReversion, f.ReversaDeMovimientoId)).ToList());

        // Stock esperado hoy = saldo del cierre + movimientos de períodos posteriores.
        var posteriores = posterioresPorArticulo.ToDictionary(
            kv => kv.Key,
            kv => kv.Value.Sum(m => m.Tipo == TipoMovimientoArticulo.Ingreso ? m.Cantidad : -m.Cantidad));

        var stocks = await _uow.Logistica.Maestros.StockArticulos.Query().AsNoTracking()
            .Where(s => s.PlantaCode == planta)
            .Select(s => new { s.ArticuloCode, s.Quantity, Costo = s.AverageCost ?? s.UnitCost })
            .ToListAsync(ct);

        var stockPorArticulo = stocks.ToDictionary(s => s.ArticuloCode);

        // El legacy dejaba Stock Y Precio_Pro del stock vivo con los valores del cierre. Aquí la cantidad NO se toca,
        // pero el costo promedio vivo se alinea: se parte del costo del cierre y se aplican los movimientos de períodos
        // posteriores con la misma regla, para que las correcciones del cierre (ajustes, Nota de Crédito 01, orden
        // cronológico) no se arrastren al mes siguiente.
        var costosNuevos = new Dictionary<string, decimal>();
        foreach (var r in resultados)
        {
            if (!stockPorArticulo.TryGetValue(r.ArticuloCode, out var vivo))
                continue;

            var costo = r.CostoPromedio;
            if (posterioresPorArticulo.TryGetValue(r.ArticuloCode, out var despues) && despues.Count > 0)
            {
                var replay = KardexValorizador.Calcular(r.Cantidad, r.CostoPromedio, despues);
                if (replay.Count > 0)
                    costo = replay[^1].CostoPromedio;
            }

            if (costo > 0m && Math.Abs(vivo.Costo - costo) > ToleranciaCosto)
                costosNuevos[r.ArticuloCode] = costo;
        }

        if (costosNuevos.Count > 0)
        {
            var codigosCosto = costosNuevos.Keys.ToList();
            var filasStock = await _uow.Logistica.Maestros.StockArticulos.Query()
                .Where(s => s.PlantaCode == planta && codigosCosto.Contains(s.ArticuloCode))
                .ToListAsync(ct);

            foreach (var fila in filasStock)
                fila.AverageCost = costosNuevos[fila.ArticuloCode];
        }

        decimal diferenciaImporte = 0;

        foreach (var articulo in stockPorArticulo.Keys.Union(saldosNuevos.Keys).Union(posteriores.Keys))
        {
            stockPorArticulo.TryGetValue(articulo, out var stock);
            saldosNuevos.TryGetValue(articulo, out var saldo);
            posteriores.TryGetValue(articulo, out var neto);

            var esperado = saldo + neto;
            var real = stock?.Quantity ?? 0m;
            var diferencia = real - esperado;

            if (Math.Abs(diferencia) < ToleranciaCantidad)
                continue;

            diferenciaImporte += diferencia * (stock?.Costo ?? 0m);

            await _uow.Logistica.Transacciones.CierreIncidencias.AddAsync(new CierreIncidencia
            {
                PlantaCode = planta,
                PeriodCode = periodo,
                ArticuloCode = articulo,
                Tipo = TipoIncidenciaCierre.DiferenciaConStock,
                Mensaje = Truncar(
                    $"El stock actual ({real:0.##}) no coincide con el saldo del Kardex ({esperado:0.##}); diferencia {diferencia:0.##}.")
            }, ct);
            incidencias++;
        }

        // ------------------------------------------------------------ 7. registrar el cierre y confirmar
        var diferenciaTotal = Math.Round(diferenciaImporte, 2, MidpointRounding.AwayFromZero);

        await _uow.Logistica.Maestros.ControlCierres.AddAsync(new ControlCierre
        {
            PlantaCode = planta,
            PeriodCode = periodo,
            ClosureDate = fin,
            DifferenceAmount = diferenciaTotal
        }, ct);

        await _uow.SaveChangesAsync(ct);

        // Si entre la lectura y este punto se registró un movimiento en el período, se revierte todo.
        var huellaFinal = await LeerHuellaAsync(planta, periodo, ct);
        if (huellaFinal != huellaInicial)
        {
            throw new InvalidOperationException(
                $"Se registraron movimientos en el período {periodo} mientras se procesaba el cierre. Intente nuevamente.");
        }

        return new CierreMensualResponse(
            planta, periodo, fin,
            conSaldo,
            resultados.Count(r => r.TuvoMovimientos),
            Math.Round(importeTotal, 2, MidpointRounding.AwayFromZero),
            documentosRevalorizados,
            incidencias,
            diferenciaTotal,
            costosNuevos.Count);
    }

    private async Task<(int Conteo, long MaxId)> LeerHuellaAsync(string planta, string periodo, CancellationToken ct)
    {
        var movimientos = _uow.Logistica.Transacciones.MovimientosArticulo.Query().AsNoTracking()
            .Where(m => m.PlantaCode == planta && m.PeriodCode == periodo);

        var conteo = await movimientos.CountAsync(ct);
        var maxId = await movimientos.OrderByDescending(m => m.Id).Select(m => m.Id).FirstOrDefaultAsync(ct);

        return (conteo, maxId);
    }

    private static string Truncar(string mensaje) => mensaje.Length <= 500 ? mensaje : mensaje[..500];
}
