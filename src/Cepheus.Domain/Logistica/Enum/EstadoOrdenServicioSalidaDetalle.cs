namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Estado de una línea del vale de salida. Legacy: dbo.MOrdenServicioDetS.Codigo_Est.
///   Pendiente = 1 (01), Procesado = 13 (13), Devuelto = 14 (14), Anulado = 4 (04).
/// Al anular una orden procesada, el legacy pasaba sus líneas procesadas a 14 (Devuelto); las que
/// no se habían procesado quedan Anuladas.
/// </summary>
public enum EstadoOrdenServicioSalidaDetalle
{
    Pendiente = 1,
    Procesado = 13,
    Devuelto = 14,
    Anulado = 4
}
