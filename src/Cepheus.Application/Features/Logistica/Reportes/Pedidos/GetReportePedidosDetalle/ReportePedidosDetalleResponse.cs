namespace Cepheus.Application.Features.Logistica.Reportes.Pedidos.GetReportePedidosDetalle;

public sealed record ReportePedidosDetalleResponse(
    DateTime FechaDesde,
    DateTime FechaHasta,
    int TotalFilas,
    IReadOnlyList<ReportePedidoDetalleFilaResponse> Filas);

public sealed record ReportePedidoDetalleFilaResponse(
    string PlantaCode,
    string PedidoCode,
    string TipoPedidoNombre,
    DateTime FechaRegistro,
    DateTime FechaEntrega,
    string TramiteNombre,
    string SubCentroCostoNombre,
    string? OrdenTrabajoCode,
    string Observaciones,
    string? Usuario,
    string? AprobadoPor,
    string Estado,
    string? ArticuloCode,
    string? DescripcionArticulo,
    string? UnidadMedidaCode,
    // Codigo_Com del legacy: orden de compra a la que se atendió el artículo.
    string? OrdenCompraCode,
    decimal? Cantidad,
    string? EstadoDetalle);
