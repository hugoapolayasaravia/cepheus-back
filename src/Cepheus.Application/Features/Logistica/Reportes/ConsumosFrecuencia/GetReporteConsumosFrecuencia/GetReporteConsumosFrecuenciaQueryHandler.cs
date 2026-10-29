using Cepheus.Application.Comun.Interfaces.UnitOfWork;
using Cepheus.Application.Features.Logistica.Reportes.Common;
using Cepheus.Domain.Logistica.Enum;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosFrecuencia.GetReporteConsumosFrecuencia;

/// <summary>Solo lectura y sin SQL dinámico: reemplaza Logi_sp_Rpt_Consumo_Frecuencia.</summary>
public sealed class GetReporteConsumosFrecuenciaQueryHandler
    : IRequestHandler<GetReporteConsumosFrecuenciaQuery, ReporteConsumosFrecuenciaResponse>
{
    // El legacy usaba "set language Español" para DATENAME; se fijan los nombres para no depender de la cultura del servidor.
    private static readonly string[] Meses =
    {
        "enero", "febrero", "marzo", "abril", "mayo", "junio",
        "julio", "agosto", "setiembre", "octubre", "noviembre", "diciembre"
    };

    private readonly IUnitOfWork _uow;

    public GetReporteConsumosFrecuenciaQueryHandler(IUnitOfWork uow) => _uow = uow;

    public async Task<ReporteConsumosFrecuenciaResponse> Handle(
        GetReporteConsumosFrecuenciaQuery request, CancellationToken ct)
    {
        var planta = ReporteRules.Normalizar(request.PlantaCode);
        var (desde, hastaExcl) = ReporteRules.Rango(request.FechaDesde, request.FechaHasta);
        var fam = ReporteRules.Filtro(request.FamiliaCode);
        var sfa = ReporteRules.Filtro(request.SubFamiliaCode);

        var q = _uow.Logistica.Transacciones.ValeDetalles.Query().AsNoTracking()
            .Where(d => d.PlantaCode == planta
                        && d.Estado == EstadoValeDetalle.Procesado
                        && d.Vale.FechaEntrega >= desde
                        && d.Vale.FechaEntrega < hastaExcl);

        if (fam is not null) q = q.Where(d => d.Articulo.SubFamilia.FamiliaCode == fam);
        if (sfa is not null) q = q.Where(d => d.Articulo.SubFamiliaCode == sfa);

        var grupos = await q
            .GroupBy(d => new
            {
                d.Vale.FechaEntrega.Year,
                d.Vale.FechaEntrega.Month,
                d.ArticuloCode,
                ArticuloNombre = d.Articulo.Name,
                d.Articulo.UnidadMedidaCode
            })
            .Select(g => new
            {
                g.Key.Year, g.Key.Month, g.Key.ArticuloCode, g.Key.ArticuloNombre, g.Key.UnidadMedidaCode,
                Cantidad = g.Sum(x => x.Cantidad),
                Precio = g.Average(x => x.Precio)
            })
            .ToListAsync(ct);

        var codigos = grupos.Select(g => g.ArticuloCode).Distinct().ToList();
        var precios = codigos.Count == 0
            ? new Dictionary<string, decimal?>()
            : await _uow.Logistica.Maestros.StockArticulos.Query().AsNoTracking()
                .Where(s => s.PlantaCode == planta && codigos.Contains(s.ArticuloCode))
                .ToDictionaryAsync(s => s.ArticuloCode, s => (decimal?)(s.AverageCost ?? s.UnitCost), ct);

        // Legacy: ORDER BY año, mes, nombre del mes.
        var filas = grupos
            .OrderBy(g => g.Year).ThenBy(g => g.Month).ThenBy(g => g.ArticuloCode)
            .Select(g => new ReporteConsumoFrecuenciaFilaResponse(
                g.Year, g.Month, Meses[g.Month - 1], g.ArticuloCode, g.ArticuloNombre, g.UnidadMedidaCode,
                g.Cantidad, ReporteRules.Redondear(g.Precio, 6),
                precios.TryGetValue(g.ArticuloCode, out var p) ? p : null))
            .ToList();

        return new ReporteConsumosFrecuenciaResponse(planta, desde, hastaExcl.AddDays(-1), filas.Count, filas);
    }
}
