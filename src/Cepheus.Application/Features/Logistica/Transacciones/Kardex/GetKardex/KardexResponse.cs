namespace Cepheus.Application.Features.Logistica.Transacciones.Kardex.GetKardex;

public sealed record KardexSaldoResponse(
    decimal Cantidad,
    decimal CostoPromedio,
    decimal Importe);

public sealed record KardexMovimientoResponse(
    long Id,
    DateTime Fecha,
    string Periodo,
    // True si el período del movimiento ya fue cerrado en la planta.
    bool PeriodoCerrado,
    string Tipo,
    string Origen,
    string DocumentoCode,
    int? ItemNumber,
    decimal CantidadIngreso,
    decimal CantidadSalida,
    // Costo con el que se registró el movimiento (provisional mientras el período está abierto).
    decimal CostoRegistrado,
    // Costo valorizado por promedio móvil en orden cronológico (el mismo que usará el cierre).
    decimal CostoValorizado,
    decimal ImporteMovimiento,
    decimal SaldoCantidad,
    decimal CostoPromedio,
    decimal SaldoImporte,
    // Movimiento anulado (y su reversión): se muestra pero no altera el saldo ni el costo.
    bool Anulado,
    bool EsReversion,
    long? ReversaDeMovimientoId,
    // El saldo queda negativo en este punto según el orden por fechas.
    bool StockNegativo,
    // Origen de 2 letras del legacy (NI, OS, VS, AI, AS, GI...). Columna "Ori" del Kardex.
    string OrigenCodigo,
    // Ajuste de inventario: el legacy lo muestra en rojo.
    bool EsAjuste,
    // Comprobante del documento (01 factura, 07 nota de crédito...). Columna "Tipo de Doc".
    string? TipoDocumentoCode,
    // Motivo de la nota de crédito (01 ajuste de precio, 02 devolución). Columna "Motivo".
    string? MotivoCode);

public sealed record KardexTotalesResponse(
    decimal CantidadIngresos,
    decimal CantidadSalidas,
    decimal ImporteIngresos,
    decimal ImporteSalidas);

public sealed record KardexResponse(
    string PlantaCode,
    string PlantaName,
    string ArticuloCode,
    string ArticuloName,
    string UnidadMedidaCode,
    string UnidadMedidaName,
    DateTime FechaDesde,
    DateTime FechaHasta,
    // Último período cerrado de la planta (AAAAMM); null si nunca se cerró uno.
    string? UltimoPeriodoCerrado,
    // Período del último saldo mensual usado como base; null si se calculó desde el primer movimiento.
    string? PeriodoBase,
    KardexSaldoResponse SaldoInicial,
    IReadOnlyList<KardexMovimientoResponse> Movimientos,
    KardexTotalesResponse Totales,
    KardexSaldoResponse SaldoFinal,
    // Stock actual del maestro (StockArticulos), solo para contraste.
    decimal StockActual,
    // True/false cuando el rango llega hasta el último movimiento y se puede comparar el saldo final contra
    // el stock actual; null si hay movimientos posteriores a FechaHasta.
    bool? ConciliaConStockActual,
    IReadOnlyList<string> Advertencias);
