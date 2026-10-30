namespace Cepheus.Application.Features.Logistica.Transacciones.Pedidos.PedidoDetalles.GetPedidoDetalles;

public sealed record PedidoDetalleResponse(
    string PlantaCode,
    string PedidoCode,
    string? ArticuloCode,
    string DescripcionArticulo,
    string UnidadMedidaCode,
    decimal PrecioArticulo,
    decimal CantidadArticulo,
    decimal TotalArticulo,
    int ItemNumber,
    string CodigoEstado,
    string? OrdenCompraCode,
    string? NombreProveedor,
    decimal Stock,
    decimal Pendiente
);