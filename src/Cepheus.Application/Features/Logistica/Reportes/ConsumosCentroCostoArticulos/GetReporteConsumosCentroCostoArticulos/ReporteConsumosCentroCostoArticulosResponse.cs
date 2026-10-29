namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosCentroCostoArticulos.GetReporteConsumosCentroCostoArticulos;

public sealed record ReporteConsumosCentroCostoArticulosResponse(
    DateTime FechaDesde,
    DateTime FechaHasta,
    int TotalFilas,
    decimal TotalImporte,
    IReadOnlyList<ReporteConsumoCentroCostoArticuloFilaResponse> Filas);

public sealed record ReporteConsumoCentroCostoArticuloFilaResponse(
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
    string TrabajadorNombre,
    string SubFamiliaCode,
    string SubFamiliaNombre,
    string FamiliaCode,
    string FamiliaNombre,
    string? OrdenTrabajoCode,
    string Estado,
    string TipoValeCode);
