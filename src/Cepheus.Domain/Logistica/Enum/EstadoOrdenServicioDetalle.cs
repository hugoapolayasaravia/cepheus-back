namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Estado de una línea de Orden de Servicio. Legacy: dbo.MOrdenServicioDetI.Codigo_Est.
///   Pendiente = 1  -> 01
///   Procesado = 13 -> 13
///   Anulado   = 4  -> 04
/// La línea nace Pendiente, pasa a Procesado con la orden y a Anulado con ella.
/// </summary>
public enum EstadoOrdenServicioDetalle
{
    Pendiente = 1,
    Procesado = 13,
    Anulado = 4
}
