using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Maestros.ControlCierres.Common;
using Cepheus.Application.Features.Logistica.Transacciones.Kardex.Common;
using Cepheus.Application.Features.Logistica.Transacciones.ReporteCierre.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Transacciones.ReporteCierre.GetReporteCierre;

/// <summary>
/// Reporte del cierre. Solo lectura sobre SaldosMensualesArticulo: sin SQL dinámico y sin tablas temporales.
/// </summary>
public sealed class GetReporteCierreQueryHandler : IRequestHandler<GetReporteCierreQuery, ReporteCierreResponse>
{
    private readonly IUnitOfWork _uow;

    public GetReporteCierreQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteCierreResponse> Handle(GetReporteCierreQuery request, CancellationToken ct)
    {
        var planta = request.PlantaCode.Trim().ToUpperInvariant();
        var actual = request.PeriodoCode.Trim();
        var comparacion = string.IsNullOrWhiteSpace(request.PeriodoComparacionCode)
            ? ControlCierreRules.PreviousPeriod(actual)
            : request.PeriodoComparacionCode.Trim();

        var origen = string.IsNullOrWhiteSpace(request.Origen)
            ? ArticuloOrigenRules.Todos
            : request.Origen.Trim().ToUpperInvariant();

        var plantaInfo = await _uow.Comunes.Plantas.Query().AsNoTracking()
            .Where(p => p.Code == planta)
            .Select(p => new { p.Name })
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException($"La planta {planta} no existe.");

        // --- los períodos deben estar cerrados (el de comparación puede ser anterior al primer cierre: saldo cero)
        var cierres = _uow.Logistica.Maestros.ControlCierres.Query().AsNoTracking()
            .Where(c => c.PlantaCode == planta);

        var cierreActual = await cierres
            .Where(c => c.PeriodCode == actual)
            .Select(c => new { c.ClosureDate, c.DifferenceAmount })
            .FirstOrDefaultAsync(ct)
            ?? throw new InvalidOperationException($"El período {actual} no está cerrado en la planta {planta}.");

        var primero = await cierres.OrderBy(c => c.PeriodCode).Select(c => c.PeriodCode).FirstAsync(ct);
        var ultimo = await cierres.OrderByDescending(c => c.PeriodCode).Select(c => c.PeriodCode).FirstAsync(ct);

        if (string.CompareOrdinal(comparacion, ultimo) > 0)
            throw new InvalidOperationException($"El período de comparación {comparacion} no está cerrado en la planta {planta}.");

        // --- filtros de artículos
        var articulos = _uow.Logistica.Maestros.Articulos.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.ArticuloCode))
        {
            var codigo = request.ArticuloCode.Trim().ToUpperInvariant();
            articulos = articulos.Where(a => a.Code == codigo);
        }

        if (!string.IsNullOrWhiteSpace(request.FamiliaDesde))
        {
            var desde = request.FamiliaDesde.Trim().ToUpperInvariant();
            var hasta = string.IsNullOrWhiteSpace(request.FamiliaHasta) ? desde : request.FamiliaHasta.Trim().ToUpperInvariant();
            articulos = articulos.Where(a =>
                string.Compare(a.SubFamilia.FamiliaCode, desde) >= 0 &&
                string.Compare(a.SubFamilia.FamiliaCode, hasta) <= 0);
        }

        if (!string.IsNullOrWhiteSpace(request.SubFamiliaDesde))
        {
            var desde = request.SubFamiliaDesde.Trim().ToUpperInvariant();
            var hasta = string.IsNullOrWhiteSpace(request.SubFamiliaHasta) ? desde : request.SubFamiliaHasta.Trim().ToUpperInvariant();
            articulos = articulos.Where(a =>
                string.Compare(a.SubFamiliaCode, desde) >= 0 &&
                string.Compare(a.SubFamiliaCode, hasta) <= 0);
        }

        var origenAplicado = ArticuloOrigenRules.Aplica(planta, origen);
        if (origenAplicado)
        {
            articulos = origen == ArticuloOrigenRules.Nacional
                ? articulos.Where(a => string.Compare(a.Code, ArticuloOrigenRules.CodigoLimite) < 0)
                : articulos.Where(a => string.Compare(a.Code, ArticuloOrigenRules.CodigoLimite) > 0);
        }

        // --- saldos de los dos períodos
        var saldos = _uow.Logistica.Transacciones.SaldosMensualesArticulo.Query().AsNoTracking()
            .Where(s => s.PlantaCode == planta && (s.PeriodCode == actual || s.PeriodCode == comparacion));

        var filas = await (
            from s in saldos
            join a in articulos on s.ArticuloCode equals a.Code
            select new
            {
                a.Code, a.Name, a.UnidadMedidaCode,
                FamiliaCode = a.SubFamilia.FamiliaCode, a.SubFamiliaCode,
                s.PeriodCode, s.Quantity, s.AverageCost
            })
            .ToListAsync(ct);

        var (reporte, totales) = ReporteCierreBuilder.Armar(
            filas.Select(f => new SaldoLeido(
                f.Code, f.Name, f.UnidadMedidaCode, f.FamiliaCode, f.SubFamiliaCode,
                f.PeriodCode, f.Quantity, f.AverageCost,
                KardexValorizador.Importe(f.Quantity, f.AverageCost))),
            actual, comparacion, request.IncluirSaldoCero);

        var incidencias = await _uow.Logistica.Transacciones.CierreIncidencias.Query().AsNoTracking()
            .CountAsync(i => i.PlantaCode == planta && i.PeriodCode == actual, ct);

        return new ReporteCierreResponse(
            planta, plantaInfo.Name,
            actual, comparacion,
            cierreActual.ClosureDate, cierreActual.DifferenceAmount, incidencias,
            FiltroOrigenAplicado: origen == ArticuloOrigenRules.Todos || origenAplicado,
            reporte, totales);
    }
}
