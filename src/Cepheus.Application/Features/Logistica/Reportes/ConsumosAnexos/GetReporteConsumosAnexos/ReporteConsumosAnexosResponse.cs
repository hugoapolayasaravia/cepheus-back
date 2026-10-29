namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosAnexos.GetReporteConsumosAnexos;

public sealed record ReporteConsumosAnexosResponse(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    int TotalFilas,
    decimal TotalImporte,
    IReadOnlyList<ReporteConsumoAnexoFilaResponse> Filas);

public sealed record ReporteConsumoAnexoFilaResponse(
    // VS = Vale de Salida, OS = salida por Orden de Servicio.
    string Fuente,
    string ValeCode,
    DateTime? FechaProceso,
    DateTime FechaEntrega,
    // Neto del vale completo: se repite en cada línea, como en el legacy.
    decimal NetoVale,
    decimal Cantidad,
    decimal Precio,
    decimal Total,
    string CentroCostoCode,
    string CentroCostoNombre,
    string SubCentroCostoCode,
    string SubCentroCostoNombre,
    string FamiliaCode,
    string FamiliaNombre,
    string SubFamiliaCode,
    string SubFamiliaNombre,
    string CuentaCode,
    string CuentaNombre,
    // Tipo de anexo del subcentro de costo.
    string? TipoAnexoCode,
    string? TipoAnexoNombre,
    // XXXXX cuando la subfamilia no tiene enlace de anexo en ese subcentro.
    string AnexoCode,
    // Nombre del anexo, o TIENE CUENTA CONTABLE DIRECTA / NO EXISTE ENLACE según el caso (igual que el legacy).
    string AnexoNombre,
    string PlantaAfectadaCode,
    string? PlantaAfectadaNombre,
    string ArticuloNombre);
