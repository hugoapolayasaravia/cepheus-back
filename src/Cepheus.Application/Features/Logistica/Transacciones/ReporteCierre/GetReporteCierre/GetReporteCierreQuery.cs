using Cepheus.Application.Features.Logistica.Transacciones.ReporteCierre.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Transacciones.ReporteCierre.GetReporteCierre;

/// <summary>
/// Reporte del cierre: saldos y costos de un período cerrado comparados con otro (por defecto, el anterior).
/// Filtros del legacy: origen Nacional/Importado, familia y subfamilia por rango, artículo.
/// </summary>
public sealed record GetReporteCierreQuery(
    string PlantaCode,
    string PeriodoCode,
    string? PeriodoComparacionCode,
    string? Origen,
    string? FamiliaDesde,
    string? FamiliaHasta,
    string? SubFamiliaDesde,
    string? SubFamiliaHasta,
    string? ArticuloCode,
    bool IncluirSaldoCero) : IRequest<ReporteCierreResponse>;

public sealed record ReporteCierreResponse(
    string PlantaCode,
    string PlantaName,
    string PeriodoActual,
    string PeriodoComparacion,
    DateTime FechaCierreActual,
    // Diferencia contra el stock que encontró el cierre del período actual, y cantidad de incidencias.
    decimal DiferenciaStockCierre,
    int IncidenciasCierre,
    // False cuando se pidió Nacional/Importado pero la planta no lo clasifica (el filtro se ignora, como el legacy).
    bool FiltroOrigenAplicado,
    IReadOnlyList<ReporteFila> Filas,
    ReporteTotales Totales);
