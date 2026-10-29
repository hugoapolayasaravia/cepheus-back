using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Kardex.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Reportes.InventarioPermanente.GetReporteInventarioPermanenteValorizado;

/// <summary>
/// Solo lectura: lee el saldo del último cierre y los movimientos de todos los artículos filtrados en una sola pasada y
/// valoriza cada artículo con KardexValorizador. Reemplaza el cursor, las tablas temporales por usuario y las
/// funciones fn_cargacierre / fn_cargaprecio del legacy.
/// </summary>
public sealed class GetReporteInventarioPermanenteValorizadoQueryHandler
    : IRequestHandler<GetReporteInventarioPermanenteValorizadoQuery, ReporteInventarioPermanenteValorizadoResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReporteInventarioPermanenteValorizadoQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteInventarioPermanenteValorizadoResponse> Handle(
        GetReporteInventarioPermanenteValorizadoQuery request, CancellationToken ct)
    {
        var planta = ReporteRules.Normalizar(request.PlantaCode);
        var desde = request.FechaDesde.Date;
        var hasta = request.FechaHasta.Date;
        var hastaExcl = hasta.AddDays(1);
        var periodoDesde = ControlCierreRules.PeriodOf(desde);

        var fam = ReporteRules.Filtro(request.FamiliaCode);
        var famHasta = ReporteRules.Filtro(request.FamiliaHastaCode);
        var sfa = ReporteRules.Filtro(request.SubFamiliaCode);
        var sfa2 = ReporteRules.Filtro(request.SubFamilia2Code);

        // Filtro de artículos compartido por saldos y movimientos.
        // Familia: BETWEEN si vienen ambas, igualdad si solo una. Subfamilia: una o dos.
        var saldosQ = _uow.Logistica.Transacciones.SaldosMensualesArticulo.Query().AsNoTracking()
            .Where(s => s.PlantaCode == planta && string.Compare(s.PeriodCode, periodoDesde) < 0);

        var movsQ = _uow.Logistica.Transacciones.MovimientosArticulo.Query().AsNoTracking()
            .Where(m => m.PlantaCode == planta && m.FechaMovimiento < hastaExcl);

        if (fam is not null)
        {
            if (famHasta is not null)
            {
                saldosQ = saldosQ.Where(s => string.Compare(s.Articulo.SubFamilia.FamiliaCode, fam) >= 0
                                             && string.Compare(s.Articulo.SubFamilia.FamiliaCode, famHasta) <= 0);
                movsQ = movsQ.Where(m => string.Compare(m.Articulo.SubFamilia.FamiliaCode, fam) >= 0
                                         && string.Compare(m.Articulo.SubFamilia.FamiliaCode, famHasta) <= 0);
            }
            else
            {
                saldosQ = saldosQ.Where(s => s.Articulo.SubFamilia.FamiliaCode == fam);
                movsQ = movsQ.Where(m => m.Articulo.SubFamilia.FamiliaCode == fam);
            }
        }

        if (sfa is not null)
        {
            if (sfa2 is not null)
            {
                saldosQ = saldosQ.Where(s => s.Articulo.SubFamiliaCode == sfa || s.Articulo.SubFamiliaCode == sfa2);
                movsQ = movsQ.Where(m => m.Articulo.SubFamiliaCode == sfa || m.Articulo.SubFamiliaCode == sfa2);
            }
            else
            {
                saldosQ = saldosQ.Where(s => s.Articulo.SubFamiliaCode == sfa);
                movsQ = movsQ.Where(m => m.Articulo.SubFamiliaCode == sfa);
            }
        }

        // Último saldo cerrado anterior al período pedido, por artículo.
        var saldos = (await saldosQ
                .Select(s => new { s.ArticuloCode, s.PeriodCode, s.Quantity, s.AverageCost })
                .ToListAsync(ct))
            .GroupBy(s => s.ArticuloCode)
            .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.PeriodCode).First());

        var movimientos = (await movsQ
                .Select(m => new
                {
                    m.Id, m.ArticuloCode, m.FechaMovimiento, m.PeriodCode, m.Tipo, m.Origen, m.Cantidad,
                    m.CostoUnitario, m.EsReversion, m.ReversaDeMovimientoId, m.DocumentoCode
                })
                .ToListAsync(ct))
            .GroupBy(m => m.ArticuloCode)
            .ToDictionary(g => g.Key, g => g.ToList());

        var codigos = saldos.Keys.Union(movimientos.Keys).OrderBy(c => c, StringComparer.Ordinal).ToList();


        var info = new Dictionary<string, (string Nombre, string Unidad)>();

        if (codigos.Count > 0)
        {
            var articulosInfo = await _uow.Logistica.Maestros.Articulos.Query().AsNoTracking()
                .Where(a => codigos.Contains(a.Code))
                .Select(a => new { a.Code, a.Name, a.UnidadMedidaCode })
                .ToListAsync(ct);

            foreach (var a in articulosInfo)
                info[a.Code] = (a.Name, a.UnidadMedidaCode);
        }


        var articulos = new List<InventarioArticuloResponse>();

        foreach (var codigo in codigos)
        {
            saldos.TryGetValue(codigo, out var baseSaldo);
            var baseCantidad = baseSaldo?.Quantity ?? 0m;
            var baseCosto = baseSaldo?.AverageCost ?? 0m;

            var entradas = (movimientos.TryGetValue(codigo, out var lista) ? lista : [])
                .Where(m => baseSaldo is null || string.CompareOrdinal(m.PeriodCode, baseSaldo.PeriodCode) > 0)
                .Select(m => new KardexEntrada(
                    m.Id, m.FechaMovimiento, m.PeriodCode, m.Tipo, m.Origen, m.Cantidad, m.CostoUnitario,
                    m.EsReversion, m.ReversaDeMovimientoId))
                .ToList();

            var lineas = KardexValorizador.Calcular(baseCantidad, baseCosto, entradas);

            var previa = lineas.LastOrDefault(l => l.Entrada.Fecha.Date < desde);
            var saldoInicial = previa is null
                ? new InventarioSaldoResponse(baseCantidad, baseCosto, KardexValorizador.Importe(baseCantidad, baseCosto))
                : new InventarioSaldoResponse(previa.SaldoCantidad, previa.CostoPromedio, previa.SaldoImporte);

            var visibles = lineas.Where(l => l.Entrada.Fecha.Date >= desde).ToList();

            // Sin saldo ni movimientos en el período no hay nada que mostrar.
            if (visibles.Count == 0 && saldoInicial.Cantidad == 0m)
                continue;

            var vigentes = visibles.Where(l => !l.Anulado).ToList();
            var ingresos = vigentes.Where(l => l.Entrada.Tipo == TipoMovimientoArticulo.Ingreso).ToList();
            var salidas = vigentes.Where(l => l.Entrada.Tipo == TipoMovimientoArticulo.Salida).ToList();

            var ultima = visibles.LastOrDefault();
            var saldoFinal = ultima is null
                ? saldoInicial
                : new InventarioSaldoResponse(ultima.SaldoCantidad, ultima.CostoPromedio, ultima.SaldoImporte);

            var documentos = movimientos.TryGetValue(codigo, out var movsArt)
                ? movsArt.ToDictionary(m => m.Id, m => m.DocumentoCode)
                : new Dictionary<long, string>();

            var detalle = request.Resumen
                ? []
                : visibles.Select(l =>
                {
                    var e = l.Entrada;
                    var esIngreso = e.Tipo == TipoMovimientoArticulo.Ingreso;
                    var doc = documentos[e.Id];
                    return new InventarioMovimientoResponse(
                        e.Fecha, doc, esIngreso ? "I" : "S", KardexOrigen.Codigo(e.Origen),
                        esIngreso ? e.Cantidad : 0m, esIngreso ? l.CostoValorizado : 0m,
                        esIngreso ? 0m : e.Cantidad, esIngreso ? 0m : l.CostoValorizado,
                        l.SaldoCantidad, l.CostoPromedio, l.SaldoImporte, l.Anulado, l.StockNegativo);
                }).ToList();

            info.TryGetValue(codigo, out var datosArt);

            articulos.Add(new InventarioArticuloResponse(
                codigo, datosArt.Nombre ?? string.Empty, datosArt.Unidad ?? string.Empty,
                saldoInicial,
                ingresos.Sum(l => l.Entrada.Cantidad),
                salidas.Sum(l => l.Entrada.Cantidad),
                ingresos.Count == 0 ? 0m : ingresos.Max(l => l.CostoValorizado),
                salidas.Count == 0 ? 0m : salidas.Max(l => l.CostoValorizado),
                ingresos.Sum(l => KardexValorizador.Importe(l.Entrada.Cantidad, l.CostoValorizado)),
                salidas.Sum(l => KardexValorizador.Importe(l.Entrada.Cantidad, l.CostoValorizado)),
                saldoFinal, detalle));
        }

        return new ReporteInventarioPermanenteValorizadoResponse(
            planta, desde, hasta, request.Resumen, articulos.Count,
            articulos.Sum(a => a.SaldoInicial.Importe),
            articulos.Sum(a => a.ImporteIngresos),
            articulos.Sum(a => a.ImporteSalidas),
            articulos.Sum(a => a.SaldoFinal.Importe),
            articulos);
    }
}
