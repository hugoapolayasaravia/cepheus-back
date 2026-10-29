namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Incidencias que detecta el cierre mensual. Reemplazan a la tabla global ERRORES del legacy
/// (que se borraba completa en cada ejecución y se mezclaba entre plantas).
/// </summary>
public enum TipoIncidenciaCierre
{
    /// <summary>El saldo queda negativo en algún punto según el orden por fechas.</summary>
    StockNegativo = 1,

    /// <summary>Hay una Nota de Crédito de ajuste de precio pero el artículo no tuvo ingresos en el período.</summary>
    AjustePrecioSinIngresos = 2,

    /// <summary>El stock del maestro no coincide con el saldo calculado más los movimientos posteriores.</summary>
    DiferenciaConStock = 3,

    /// <summary>Una salida quedó valorizada a costo cero.</summary>
    SalidaSinCosto = 4,

    /// <summary>
    /// El cierre del origen revalorizó una transferencia pero el período del destino ya estaba cerrado: el ingreso
    /// del destino conserva el costo anterior.
    /// </summary>
    TransferenciaDestinoCerrado = 5
}
