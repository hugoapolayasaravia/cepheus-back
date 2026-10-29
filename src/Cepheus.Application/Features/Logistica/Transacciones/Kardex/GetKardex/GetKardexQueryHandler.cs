using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Kardex.Common;
using Cepheus.Application.Features.Logistica.Transacciones.NotaIngresos.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.Kardex.GetKardex;

/// <summary>
/// Arma el Kardex solo con lecturas: el saldo de apertura sale del último cierre y los saldos corridos se
/// calculan en memoria, en orden cronológico, sobre los movimientos de UN artículo. No usa tablas temporales
/// ni escribe nada, por eso varios usuarios pueden consultarlo al mismo tiempo sin afectarse
/// (el legacy cargaba MOVIMIENTOS/ERRORES, tablas globales que se borraban en cada consulta).
/// </summary>
public sealed class GetKardexQueryHandler : IRequestHandler<GetKardexQuery, KardexResponse>
{
    private const decimal Tolerancia = 0.000001m;

    private readonly IUnitOfWork _uow;

    public GetKardexQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<KardexResponse> Handle(GetKardexQuery request, CancellationToken ct)
    {
        var planta = request.PlantaCode.Trim().ToUpperInvariant();
        var articuloCode = request.ArticuloCode.Trim().ToUpperInvariant();
        var desde = request.FechaDesde.Date;
        var hasta = request.FechaHasta.Date;

        var plantaInfo = await _uow.Comunes.Plantas.Query().AsNoTracking()
            .Where(p => p.Code == planta)
            .Select(p => new { p.Name })
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"La planta {planta} no existe.");

        var articuloInfo = await _uow.Logistica.Maestros.Articulos.Query().AsNoTracking()
            .Where(a => a.Code == articuloCode)
            .Select(a => new { a.Name, a.UnidadMedidaCode, UnidadMedidaName = a.UnidadMedida.Name })
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"El artículo {articuloCode} no existe.");

        var periodoDesde = ControlCierreRules.PeriodOf(desde);
        var periodoHasta = ControlCierreRules.PeriodOf(hasta);
        var ultimoCerrado = await ControlCierreRules.GetLastClosedPeriodAsync(_uow, planta, ct);

        // Dos intentos: si entre leer el stock y leer los movimientos otro usuario confirma un movimiento,
        // la comparación podría dar una diferencia falsa; se vuelve a leer una vez antes de avisar.
        Lectura lectura = default!;
        List<KardexLinea> lineas = default!;
        bool? concilia = null;

        for (var intento = 1; intento <= 2; intento++)
        {
            lectura = await LeerAsync(planta, articuloCode, periodoDesde, periodoHasta, hasta, ct);

            lineas = KardexValorizador.Calcular(
                lectura.BaseCantidad, lectura.BaseCosto,
                lectura.Movimientos.Select(m => m.Entrada)).ToList();

            if (lectura.HayPosteriores)
            {
                concilia = null;
                break;
            }

            var saldoFinal = lineas.Count > 0 ? lineas[^1].SaldoCantidad : lectura.BaseCantidad;
            concilia = saldoFinal == lectura.StockActual;

            if (concilia == true)
                break;
        }

        // Saldo inicial = estado justo antes de la primera fecha pedida.
        var previa = lineas.LastOrDefault(l => l.Entrada.Fecha.Date < desde);
        var saldoInicial = previa is null
            ? new KardexSaldoResponse(
                lectura.BaseCantidad, lectura.BaseCosto,
                KardexValorizador.Importe(lectura.BaseCantidad, lectura.BaseCosto))
            : new KardexSaldoResponse(previa.SaldoCantidad, previa.CostoPromedio, previa.SaldoImporte);

        var visibles = lineas.Where(l => l.Entrada.Fecha.Date >= desde).ToList();

        var (comprobantes, motivos) = await LeerComprobantesAsync(planta, visibles, lectura.Detalle, ct);

        var movimientos = visibles.Select(l =>
        {
            var e = l.Entrada;
            var info = lectura.Detalle[e.Id];
            var esIngreso = e.Tipo == TipoMovimientoArticulo.Ingreso;
            comprobantes.TryGetValue((e.Origen, info.DocumentoCode), out var tipoDoc);
            motivos.TryGetValue((e.Origen, info.DocumentoCode), out var motivo);

            return new KardexMovimientoResponse(
                e.Id,
                e.Fecha,
                e.PeriodCode,
                ultimoCerrado is not null && string.CompareOrdinal(e.PeriodCode, ultimoCerrado) <= 0,
                e.Tipo.ToString(),
                e.Origen.ToString(),
                info.DocumentoCode,
                info.ItemNumber,
                esIngreso ? e.Cantidad : 0m,
                esIngreso ? 0m : e.Cantidad,
                e.CostoUnitario,
                l.CostoValorizado,
                KardexValorizador.Importe(e.Cantidad, l.CostoValorizado),
                l.SaldoCantidad,
                l.CostoPromedio,
                l.SaldoImporte,
                l.Anulado,
                e.EsReversion,
                e.ReversaDeMovimientoId,
                l.StockNegativo,
                KardexOrigen.Codigo(e.Origen),
                KardexOrigen.EsAjuste(e.Origen),
                tipoDoc,
                motivo);
        }).ToList();

        var vigentes = visibles.Where(l => !l.Anulado).ToList();
        var totales = new KardexTotalesResponse(
            vigentes.Where(l => l.Entrada.Tipo == TipoMovimientoArticulo.Ingreso).Sum(l => l.Entrada.Cantidad),
            vigentes.Where(l => l.Entrada.Tipo == TipoMovimientoArticulo.Salida).Sum(l => l.Entrada.Cantidad),
            vigentes.Where(l => l.Entrada.Tipo == TipoMovimientoArticulo.Ingreso)
                .Sum(l => KardexValorizador.Importe(l.Entrada.Cantidad, l.CostoValorizado)),
            vigentes.Where(l => l.Entrada.Tipo == TipoMovimientoArticulo.Salida)
                .Sum(l => KardexValorizador.Importe(l.Entrada.Cantidad, l.CostoValorizado)));

        var ultima = lineas.LastOrDefault(l => l.Entrada.Fecha.Date <= hasta);
        var saldoFinalResp = ultima is null
            ? saldoInicial
            : new KardexSaldoResponse(ultima.SaldoCantidad, ultima.CostoPromedio, ultima.SaldoImporte);

        var advertencias = await ArmarAdvertenciasAsync(
            planta, articuloCode, desde, hasta, periodoHasta, lectura, lineas, concilia, ct);

        return new KardexResponse(
            planta, plantaInfo.Name,
            articuloCode, articuloInfo.Name,
            articuloInfo.UnidadMedidaCode, articuloInfo.UnidadMedidaName,
            desde, hasta,
            ultimoCerrado,
            lectura.BasePeriodo,
            saldoInicial,
            movimientos,
            totales,
            saldoFinalResp,
            lectura.StockActual,
            concilia,
            advertencias);
    }

    /// <summary>
    /// Comprobante y motivo de los documentos que los tienen (Nota de Ingreso, Importación, Nota de Crédito y
    /// Orden de Servicio). Reemplaza las columnas TIPO_DOC / MOTIVO que el SP legado armaba en MKardex.
    /// </summary>
    private async Task<(Dictionary<(OrigenMovimientoArticulo, string), string?> Comprobantes,
                        Dictionary<(OrigenMovimientoArticulo, string), string?> Motivos)>
        LeerComprobantesAsync(
            string planta,
            IReadOnlyList<KardexLinea> lineas,
            Dictionary<long, DetalleMovimiento> detalle,
            CancellationToken ct)
    {
        var comprobantes = new Dictionary<(OrigenMovimientoArticulo, string), string?>();
        var motivos = new Dictionary<(OrigenMovimientoArticulo, string), string?>();

        var origenesNi = new[]
        {
            OrigenMovimientoArticulo.NotaIngreso,
            OrigenMovimientoArticulo.NotaCredito,
            OrigenMovimientoArticulo.Importacion,
            OrigenMovimientoArticulo.TransferenciaIngreso
        };

        var codigosNi = lineas
            .Where(l => origenesNi.Contains(l.Entrada.Origen))
            .Select(l => detalle[l.Entrada.Id].DocumentoCode).Distinct().ToList();

        if (codigosNi.Count > 0)
        {
            var notas = await _uow.Logistica.Transacciones.NotaIngresos.Query().AsNoTracking()
                .Where(n => n.PlantaCode == planta && codigosNi.Contains(n.Code))
                .Select(n => new { n.Code, n.ComprobantePagoCode, n.MotivoDevolucionCode })
                .ToListAsync(ct);

            foreach (var linea in lineas.Where(l => origenesNi.Contains(l.Entrada.Origen)))
            {
                var nota = notas.FirstOrDefault(n => n.Code == detalle[linea.Entrada.Id].DocumentoCode);
                if (nota is null) continue;

                comprobantes[(linea.Entrada.Origen, nota.Code)] = nota.ComprobantePagoCode;
                motivos[(linea.Entrada.Origen, nota.Code)] = nota.MotivoDevolucionCode;
            }
        }

        var codigosOs = lineas
            .Where(l => l.Entrada.Origen == OrigenMovimientoArticulo.OrdenServicio)
            .Select(l => detalle[l.Entrada.Id].DocumentoCode).Distinct().ToList();

        if (codigosOs.Count > 0)
        {
            var ordenes = await _uow.Logistica.Transacciones.OrdenesServicio.Query().AsNoTracking()
                .Where(o => o.PlantaCode == planta && codigosOs.Contains(o.Code))
                .Select(o => new { o.Code, o.ComprobantePagoCode })
                .ToListAsync(ct);

            foreach (var orden in ordenes)
                comprobantes[(OrigenMovimientoArticulo.OrdenServicio, orden.Code)] = orden.ComprobantePagoCode;
        }

        return (comprobantes, motivos);
    }

    private async Task<Lectura> LeerAsync(
        string planta, string articulo, string periodoDesde, string periodoHasta, DateTime hasta, CancellationToken ct)
    {
        var stock = await _uow.Logistica.Maestros.StockArticulos.Query().AsNoTracking()
            .Where(s => s.PlantaCode == planta && s.ArticuloCode == articulo)
            .Select(s => new { s.Quantity })
            .FirstOrDefaultAsync(ct);

        // Base: último saldo mensual cerrado anterior al período pedido (si existe).
        var baseSaldo = await _uow.Logistica.Transacciones.SaldosMensualesArticulo.Query().AsNoTracking()
            .Where(s => s.PlantaCode == planta && s.ArticuloCode == articulo
                        && string.Compare(s.PeriodCode, periodoDesde) < 0)
            .OrderByDescending(s => s.PeriodCode)
            .Select(s => new { s.PeriodCode, s.Quantity, s.AverageCost })
            .FirstOrDefaultAsync(ct);

        var query = _uow.Logistica.Transacciones.MovimientosArticulo.Query().AsNoTracking()
            .Where(m => m.PlantaCode == planta && m.ArticuloCode == articulo && m.FechaMovimiento <= hasta);

        if (baseSaldo is not null)
        {
            var basePeriodo = baseSaldo.PeriodCode;
            query = query.Where(m => string.Compare(m.PeriodCode, basePeriodo) > 0);
        }

        var filas = await query
            .Select(m => new
            {
                m.Id, m.FechaMovimiento, m.PeriodCode, m.Tipo, m.Origen, m.Cantidad, m.CostoUnitario,
                m.EsReversion, m.ReversaDeMovimientoId, m.DocumentoCode, m.ItemNumber
            })
            .ToListAsync(ct);

        var hayPosteriores = await _uow.Logistica.Transacciones.MovimientosArticulo.Query().AsNoTracking()
            .AnyAsync(m => m.PlantaCode == planta && m.ArticuloCode == articulo && m.FechaMovimiento > hasta, ct);

        var saldosPeriodo = await _uow.Logistica.Transacciones.SaldosMensualesArticulo.Query().AsNoTracking()
            .Where(s => s.PlantaCode == planta && s.ArticuloCode == articulo
                        && string.Compare(s.PeriodCode, periodoDesde) >= 0
                        && string.Compare(s.PeriodCode, periodoHasta) <= 0)
            .Select(s => new SaldoCerrado(s.PeriodCode, s.Quantity, s.AverageCost))
            .ToListAsync(ct);

        var movimientos = filas
            .Select(f => new MovimientoLeido(
                new KardexEntrada(
                    f.Id, f.FechaMovimiento, f.PeriodCode, f.Tipo, f.Origen,
                    f.Cantidad, f.CostoUnitario, f.EsReversion, f.ReversaDeMovimientoId),
                f.DocumentoCode, f.ItemNumber))
            .ToList();

        return new Lectura(
            baseSaldo?.PeriodCode,
            baseSaldo?.Quantity ?? 0m,
            baseSaldo?.AverageCost ?? 0m,
            movimientos,
            movimientos.ToDictionary(m => m.Entrada.Id, m => new DetalleMovimiento(m.DocumentoCode, m.ItemNumber)),
            stock?.Quantity ?? 0m,
            hayPosteriores,
            saldosPeriodo);
    }

    private async Task<IReadOnlyList<string>> ArmarAdvertenciasAsync(
        string planta, string articulo, DateTime desde, DateTime hasta, string periodoHasta,
        Lectura lectura, IReadOnlyList<KardexLinea> lineas, bool? concilia, CancellationToken ct)
    {
        var advertencias = new List<string>();

        if (concilia == false)
        {
            var saldoFinal = lineas.Count > 0 ? lineas[^1].SaldoCantidad : lectura.BaseCantidad;
            advertencias.Add(
                $"El saldo del Kardex ({saldoFinal:0.##}) no coincide con el stock actual ({lectura.StockActual:0.##}). " +
                "Si el artículo tiene stock anterior al Kardex, falta ejecutar el script de apertura.");
        }

        var negativos = lineas.Count(l => !l.Anulado && l.StockNegativo && l.Entrada.Fecha.Date >= desde);
        if (negativos > 0)
        {
            advertencias.Add(
                $"{negativos} movimiento(s) dejan el stock negativo según el orden de fechas " +
                "(salidas con fecha anterior al ingreso que las cubre).");
        }

        // El período ya cerrado debe dar el mismo saldo que el Kardex recalculado.
        var finDeMesCompleto = hasta.Date == new DateTime(hasta.Year, hasta.Month, 1).AddMonths(1).AddDays(-1);

        foreach (var saldo in lectura.SaldosCerrados.OrderBy(s => s.PeriodCode))
        {
            if (saldo.PeriodCode == periodoHasta && !finDeMesCompleto)
                continue; // el rango corta el período a la mitad: no es comparable

            var fin = lineas.LastOrDefault(l => string.CompareOrdinal(l.Entrada.PeriodCode, saldo.PeriodCode) <= 0);
            var cantidad = fin?.SaldoCantidad ?? lectura.BaseCantidad;
            var costo = fin?.CostoPromedio ?? lectura.BaseCosto;

            if (Math.Abs(cantidad - saldo.Quantity) > Tolerancia || Math.Abs(costo - saldo.AverageCost) > Tolerancia)
            {
                advertencias.Add(
                    $"El cierre {saldo.PeriodCode} guardó {saldo.Quantity:0.##} a costo {saldo.AverageCost:0.######}, " +
                    $"pero el Kardex recalculado da {cantidad:0.##} a costo {costo:0.######}.");
            }
        }

        // Nota de Crédito por ajuste de precio (motivo 01): no mueve stock pero el cierre promedia el mes completo.
        var hayAjustePrecio = await _uow.Logistica.Transacciones.NotaIngresos.Query().AsNoTracking()
            .AnyAsync(n =>
                n.PlantaCode == planta &&
                n.ComprobantePagoCode == NotaIngresoRules.ComprobanteNotaCredito &&
                n.MotivoDevolucionCode == NotaIngresoRules.MotivoAjuste &&
                n.Estado == EstadoNotaIngreso.Procesado &&
                n.FechaRecepcion >= desde && n.FechaRecepcion <= hasta &&
                n.Detalles.Any(d => d.ArticuloCode == articulo), ct);

        if (hayAjustePrecio)
        {
            advertencias.Add(
                "Hay una Nota de Crédito por ajuste de precio (motivo 01) en el rango: el costo promedio definitivo " +
                "del mes lo fija el cierre mensual y puede diferir del costo mostrado.");
        }

        return advertencias;
    }

    private sealed record DetalleMovimiento(string DocumentoCode, int? ItemNumber);
    private sealed record MovimientoLeido(KardexEntrada Entrada, string DocumentoCode, int? ItemNumber);
    private sealed record SaldoCerrado(string PeriodCode, decimal Quantity, decimal AverageCost);

    private sealed record Lectura(
        string? BasePeriodo,
        decimal BaseCantidad,
        decimal BaseCosto,
        List<MovimientoLeido> Movimientos,
        Dictionary<long, DetalleMovimiento> Detalle,
        decimal StockActual,
        bool HayPosteriores,
        List<SaldoCerrado> SaldosCerrados);
}
