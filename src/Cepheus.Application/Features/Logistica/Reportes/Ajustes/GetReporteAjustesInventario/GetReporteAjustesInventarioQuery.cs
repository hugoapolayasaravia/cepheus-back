using MediatR;

namespace Cepheus.Application.Features.Logistica.Reportes.Ajustes.GetReporteAjustesInventario;

/// <summary>
/// Ajustes de Inventario procesados (legacy Logi_sp_Rpt_Ajuste), por fecha de entrega.
/// </summary>
/// <param name="Tipo">null o T = todos; Sobrante (0) o Faltante (1).</param>
public sealed record GetReporteAjustesInventarioQuery(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    string? Tipo) : IRequest<ReporteAjustesInventarioResponse>;
