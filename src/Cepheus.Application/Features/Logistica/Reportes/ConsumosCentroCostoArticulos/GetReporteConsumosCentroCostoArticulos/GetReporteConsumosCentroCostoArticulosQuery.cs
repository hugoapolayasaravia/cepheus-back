using MediatR;

namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosCentroCostoArticulos.GetReporteConsumosCentroCostoArticulos;

/// <summary>
/// Consumo por centro de costo, subcentro y artículo (legacy Logi_sp_Rpt_Consumos_CC_SCC_Articulos, que llegó
/// duplicado como "Consulta de Vales" y "Consumo por Artículos Cc Scc"). Los filtros en null equivalen a "T".
/// </summary>
/// <param name="TipoFecha">E = entrega (por defecto), S = proceso, A = aprobación.</param>
/// <param name="ArticuloCode">Coincidencia parcial sobre el código del artículo (LIKE %x%).</param>
/// <param name="EstadoDetalleCode">Estado del detalle (Codigo_est del detalle, ej. 13 = procesado). null = todos.</param>
/// <param name="EstadoCode">Estado de la cabecera del vale. null = todos.</param>
/// <param name="TipoOrdenCode">Tipo de orden de trabajo (Codigo_TOT).</param>
/// <param name="Fuente">null o T = Vales y Órdenes de Servicio; N = solo Vales; S = solo salidas de Orden de Servicio.</param>
public sealed record GetReporteConsumosCentroCostoArticulosQuery(
    string? PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    string? TipoFecha,
    string? CentroCostoCode,
    string? SubCentroCostoCode,
    string? ArticuloCode,
    string? FamiliaCode,
    string? SubFamiliaCode,
    string? TrabajadorCode,
    string? TipoValeCode,
    int? EstadoDetalleCode,
    int? EstadoCode,
    string? PlantaAfectadaCode,
    string? TipoOrdenCode,
    string? Fuente) : IRequest<ReporteConsumosCentroCostoArticulosResponse>;
