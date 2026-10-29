namespace Cepheus.Application.Features.Logistica.Reportes.Consumos.GetReporteConsumosProcesados;

public sealed record ReporteConsumosProcesadosResponse(
    DateTime FechaDesde,
    DateTime FechaHasta,
    int TotalFilas,
    decimal TotalImporte,
    IReadOnlyList<ReporteConsumoProcesadoFilaResponse> Filas);

public sealed record ReporteConsumoProcesadoFilaResponse(
    // VS = Vale de Salida, OS = salida por Orden de Servicio.
    string Fuente,
    string ValeCode,
    DateTime Fecha,
    string PlantaAfectadaCode,
    string? PlantaAfectadaNombre,
    string? CentroCostoCode,
    string? CentroCostoNombre,
    string SubCentroCostoCode,
    string? SubCentroCostoNombre,
    string ArticuloCode,
    string ArticuloNombre,
    string UnidadMedidaCode,
    decimal Cantidad,
    decimal PrecioPromedio,
    decimal Total,
    string SubFamiliaCode,
    string SubFamiliaNombre,
    string FamiliaCode,
    string FamiliaNombre);
