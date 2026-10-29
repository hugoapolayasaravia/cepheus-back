using MediatR;

namespace Cepheus.Application.Features.Logistica.Reportes.Consumos.GetReporteConsumosProcesados;

/// <summary>
/// Consumos procesados (legacy Logi_sp_Rpt_Consumo): detalle procesado de Vales de Salida y de salidas por
/// Orden de Servicio. Los filtros en null equivalen a "T" (todos).
/// </summary>
/// <param name="TipoFecha">E = fecha de entrega (por defecto), S = fecha de proceso, A = fecha de aprobación.</param>
/// <param name="Fuente">null o T = Vales y Órdenes de Servicio; N = solo Vales; S = solo salidas de Orden de Servicio.</param>
/// <param name="TipoValeCode">Origen del vale (Codigo_tva).</param>
/// <param name="PlantaAfectadaCode">Planta afectada por el consumo (Plt_Afe).</param>
public sealed record GetReporteConsumosProcesadosQuery(
    string? PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    string? TipoFecha,
    string? CentroCostoCode,
    string? SubCentroCostoCode,
    string? TipoValeCode,
    string? Fuente,
    string? PlantaAfectadaCode,
    string? FamiliaCode,
    string? SubFamiliaCode) : IRequest<ReporteConsumosProcesadosResponse>;
