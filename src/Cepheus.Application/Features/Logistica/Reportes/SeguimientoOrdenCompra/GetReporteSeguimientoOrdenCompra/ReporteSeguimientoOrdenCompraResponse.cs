namespace Cepheus.Application.Features.Logistica.Reportes.SeguimientoOrdenCompra.GetReporteSeguimientoOrdenCompra;

public sealed record ReporteSeguimientoOrdenCompraResponse(
    string PlantaCode,
    int TotalFilas,
    IReadOnlyList<ReporteSeguimientoOrdenCompraFilaResponse> Filas);

/// <summary>
/// Una fila por (artículo de la orden, pedido de origen, nota de ingreso). Si el artículo aún no tiene ingresos,
/// los campos de nota de ingreso quedan vacíos (0 / texto vacío), como en el legacy.
/// </summary>
public sealed record ReporteSeguimientoOrdenCompraFilaResponse(
    string ProveedorCode,
    string? ProveedorNombre,
    string PlantaCode,
    string OrdenCompraCode,
    DateTime FechaRegistro,
    string MonedaCode,
    string ArticuloCode,
    string ArticuloNombre,
    string UnidadMedidaCode,
    decimal CantidadOrdenada,
    decimal CantidadEntregada,
    decimal Precio,
    // Guía de la línea de la Nota de Ingreso (la Guía de Ingreso como documento aún no existe en el repo).
    string GuiaCode,
    string NotaIngresoCode,
    decimal CantidadNotaIngreso,
    decimal PrecioNotaIngreso,
    decimal TotalNotaIngreso,
    // Primeras 3 letras del estado de la orden (SUBSTRING(descripcion_est,1,3) del legacy).
    string Estado,
    decimal Stock,
    string? PedidoCode);
