namespace Cepheus.Application.Features.Logistica.Reportes.Compras.GetReporteCompras;

public sealed record ReporteComprasResponse(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    int TotalFilas,
    decimal TotalSoles,
    IReadOnlyList<ReporteCompraFilaResponse> Filas);

/// <summary>Una fila por artículo del documento. Los importes de cabecera se repiten en cada fila, como en el legacy.</summary>
public sealed record ReporteCompraFilaResponse(
    string PlantaCode,
    // NI = Nota de Ingreso, OS = Orden de Servicio (SAtencion del legacy).
    string Atencion,
    string DocumentoCode,
    DateTime FechaEmision,
    DateTime FechaRecepcion,
    string ComprobanteCode,
    string ProveedorCode,
    string? ProveedorRuc,
    string? ProveedorNombre,
    string Estado,
    decimal Igv,
    decimal Neto,
    decimal TotalDocumento,
    decimal NoGravable,
    decimal Renta,
    decimal Fonavi,
    decimal Servicio,
    decimal IgvExterior,
    string ArticuloCode,
    string ArticuloNombre,
    decimal Cantidad,
    decimal Precio,
    decimal Total,
    decimal TotalSoles,
    string MonedaCode,
    decimal TipoCambio,
    string? NumeroDocumento,
    // Codigo_Com del legacy: orden de compra de origen (solo Notas de Ingreso).
    string? OrdenCompraCode,
    string? NumeroGuia);
