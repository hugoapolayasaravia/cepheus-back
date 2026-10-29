namespace Cepheus.Application.Features.Logistica.Reportes.Ajustes.GetReporteAjustesInventario;

public sealed record ReporteAjustesInventarioResponse(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    int TotalFilas,
    decimal TotalImporte,
    IReadOnlyList<ReporteAjusteInventarioFilaResponse> Filas);

public sealed record ReporteAjusteInventarioFilaResponse(
    string PlantaCode,
    string AjusteCode,
    DateTime FechaEntrega,
    decimal NetoAjuste,
    string? Usuario,
    string ArticuloCode,
    string ArticuloNombre,
    string UnidadMedidaCode,
    decimal Cantidad,
    decimal Precio,
    decimal Total,
    string Tipo);
