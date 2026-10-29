namespace Cepheus.Application.Features.Logistica.Reportes.InventarioPermanente.GetReporteInventarioPermanenteValorizado;

public sealed record ReporteInventarioPermanenteValorizadoResponse(
    string PlantaCode,
    DateTime FechaDesde,
    DateTime FechaHasta,
    bool Resumen,
    int TotalArticulos,
    decimal ImporteInicial,
    decimal ImporteIngresos,
    decimal ImporteSalidas,
    decimal ImporteFinal,
    IReadOnlyList<InventarioArticuloResponse> Articulos);

public sealed record InventarioSaldoResponse(decimal Cantidad, decimal CostoPromedio, decimal Importe);

public sealed record InventarioArticuloResponse(
    string ArticuloCode,
    string ArticuloNombre,
    string UnidadMedidaCode,
    InventarioSaldoResponse SaldoInicial,
    decimal CantidadIngresos,
    decimal CantidadSalidas,
    // Mayor costo de ingreso y de salida del período (max(Precio_Ing) y max(Precio_Sal) del resumen legacy).
    decimal MaxCostoIngreso,
    decimal MaxCostoSalida,
    // Importe valorizado de los ingresos y de las salidas vigentes (no anuladas) del período.
    decimal ImporteIngresos,
    decimal ImporteSalidas,
    InventarioSaldoResponse SaldoFinal,
    // Vacío cuando se pide el resumen.
    IReadOnlyList<InventarioMovimientoResponse> Movimientos);

public sealed record InventarioMovimientoResponse(
    DateTime Fecha,
    string DocumentoCode,
    // I = ingreso, S = salida.
    string Tipo,
    // Código de origen del movimiento (NI, OS, VS, AI, AS, ...), el mismo del Kardex.
    string Origen,
    decimal Ingreso,
    decimal CostoIngreso,
    decimal Salida,
    decimal CostoSalida,
    decimal Saldo,
    decimal CostoPromedio,
    decimal ImporteSaldo,
    bool Anulado,
    bool StockNegativo);
