using MediatR;

namespace Cepheus.Application.Features.Logistica.Reportes.SeguimientoOrdenCompra.GetReporteSeguimientoOrdenCompra;

/// <summary>
/// Seguimiento de Orden de Compra (legacy Logi_sp_Rpt_Seguimiento_OC_GI_NI): por cada artículo de la orden, el pedido
/// de origen y las Notas de Ingreso procesadas que lo atendieron, con el stock actual.
/// FechaDesde/FechaHasta filtran por la fecha de registro de la orden; si falta una de las dos no se filtra por fecha.
/// Los filtros en null equivalen a "T" (todos).
/// </summary>
public sealed record GetReporteSeguimientoOrdenCompraQuery(
    string PlantaCode,
    string? OrdenCompraCode,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    string? ProveedorCode,
    string? ArticuloCode,
    string? FamiliaCode,
    string? SubFamiliaCode) : IRequest<ReporteSeguimientoOrdenCompraResponse>;
