namespace Cepheus.Application.Features.Logistica.Reportes.ComprasAdm.GetReporteComprasAdm;

public sealed record ReporteComprasAdmResponse(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    int TotalFilas,
    decimal TotalSoles,
    IReadOnlyList<ReporteCompraAdmFilaResponse> Filas);

public sealed record ReporteCompraAdmFilaResponse(
    string OrdenServicioCode,
    string ComprobanteAbreviado,
    DateTime FechaRecepcion,
    string ProveedorCode,
    string? ProveedorRuc,
    string? ProveedorNombre,
    string CompradorNombre,
    string MonedaCode,
    string FormaPagoNombre,
    int DiasPago,
    DateTime FechaPago,
    string Estado,
    decimal Igv,
    decimal Neto,
    decimal Monto,
    decimal NoGravable,
    decimal Renta,
    int Item,
    string? Glosa,
    string UnidadMedidaCode,
    decimal Cantidad,
    decimal Precio,
    decimal Descuento,
    decimal Total,
    string? NumeroDocumento);
