namespace Cepheus.Application.Features.Logistica.Reportes.ConsumosFrecuencia.GetReporteConsumosFrecuencia;

public sealed record ReporteConsumosFrecuenciaResponse(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    int TotalFilas,
    IReadOnlyList<ReporteConsumoFrecuenciaFilaResponse> Filas);

public sealed record ReporteConsumoFrecuenciaFilaResponse(
    int Anio,
    int Mes,
    string MesNombre,
    string ArticuloCode,
    string ArticuloNombre,
    string UnidadMedidaCode,
    decimal Cantidad,
    // Promedio simple de los precios de las líneas del mes (AVG(Precio_Art) del legacy).
    decimal PrecioPromedioConsumo,
    // Costo promedio vigente del artículo en la planta (Stock_Plt_Art.Precio_Pro); null si no tiene stock registrado.
    decimal? PrecioPromedioStock);
