namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Estado de la cabecera de una Nota de Ingreso.
/// Legacy: dbo.MNotaIngresoRes.Codigo_Est
/// (01 Pendiente, 13 Procesado, 11 Cerrado, 04 Anulado).
///
/// Se persiste con el valor ordinal (mismo criterio que EstadoOrdenCompra /
/// EstadoPedido); el mapeo al código legacy es:
///   Pendiente = 0  -> 01
///   Procesado = 1  -> 13
///   Cerrado   = 2  -> 11
///   Anulado   = 3  -> 04
///
/// En la práctica el legacy graba la nota directamente como Procesado (13)
/// y solo la anula (04); Pendiente y Cerrado se conservan por continuidad.
/// </summary>
public enum EstadoNotaIngreso
{
    Pendiente = 1,
    Procesado = 13,
    Cerrado = 11,
    Anulado = 4
}
