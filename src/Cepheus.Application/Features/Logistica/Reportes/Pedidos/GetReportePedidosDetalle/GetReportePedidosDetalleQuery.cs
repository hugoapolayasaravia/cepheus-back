using MediatR;

namespace Cepheus.Application.Features.Logistica.Reportes.Pedidos.GetReportePedidosDetalle;

/// <summary>
/// Orden de Pedidos con su detalle (legacy Logi_sp_Rpt_Pedidos_Detalle). Los pedidos sin detalle también aparecen.
/// Los filtros en null equivalen a "T" (todos). FechaDesde/FechaHasta filtran por la fecha de registro del pedido.
/// </summary>
/// <param name="EstadoCode">Estado del pedido (ej. 1 pendiente, 9 aprobado, 4 anulado).</param>
/// <param name="Usuario">Usuario que registró el pedido.</param>
public sealed record GetReportePedidosDetalleQuery(
    string? PlantaCode,
    string? SubCentroCostoCode,
    int? EstadoCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    string? Usuario,
    string? TipoPedidoCode) : IRequest<ReportePedidosDetalleResponse>;
