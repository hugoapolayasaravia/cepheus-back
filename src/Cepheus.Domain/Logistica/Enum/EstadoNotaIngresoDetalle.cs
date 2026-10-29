namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Estado de una línea de Nota de Ingreso.
/// Legacy: dbo.MNotaIngresoDet.Codigo_Est (13 Procesado, 04 Anulado).
///   Procesado = 0 -> 13
///   Anulado   = 1 -> 04
/// (mismo criterio que EstadoPedidoDetalle: el valor por defecto es el activo).
/// </summary>
public enum EstadoNotaIngresoDetalle
{
    Procesado = 13,
    Anulado = 4
}
