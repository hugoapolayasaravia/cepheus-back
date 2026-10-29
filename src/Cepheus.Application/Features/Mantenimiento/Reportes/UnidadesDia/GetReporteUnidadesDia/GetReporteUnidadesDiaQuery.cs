using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Reportes.UnidadesDia.GetReporteUnidadesDia;

/// <summary>
/// Listado de unidades para la captura diaria de horómetro y paradas (legacy
/// Logi_sp_Listado_SCcosto_PlantaAfectada). Devuelve cada unidad (subcentro de costo marcado como unidad) con
/// su registro del día, o los valores por defecto si aún no tiene. Los filtros en null equivalen a "T" (todos).
/// </summary>
/// <param name="CentroCostoCode">Centro de costo del subcentro (legacy @cc).</param>
/// <param name="PlantaCode">Planta del subcentro (legacy @plt).</param>
/// <param name="Fecha">Día a consultar (legacy @fi_prg).</param>
public sealed record GetReporteUnidadesDiaQuery(
    string? CentroCostoCode,
    string? PlantaCode,
    DateTime Fecha) : IRequest<ReporteUnidadesDiaResponse>;
