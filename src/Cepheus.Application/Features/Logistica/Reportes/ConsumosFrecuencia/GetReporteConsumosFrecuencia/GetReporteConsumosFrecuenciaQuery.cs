using MediatR;

namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosFrecuencia.GetReporteConsumosFrecuencia;

/// <summary>
/// Consumos por frecuencia (legacy Logi_sp_Rpt_Consumo_Frecuencia): por mes y artículo, cantidad consumida en Vales de
/// Salida procesados, precio promedio y precio promedio de stock actual. Filtra por fecha de entrega del vale.
/// Los filtros en null equivalen a "T" (todos).
/// </summary>
public sealed record GetReporteConsumosFrecuenciaQuery(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    string? FamiliaCode,
    string? SubFamiliaCode) : IRequest<ReporteConsumosFrecuenciaResponse>;
