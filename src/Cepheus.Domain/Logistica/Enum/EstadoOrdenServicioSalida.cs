namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Estado del vale de salida de una Orden de Servicio. Legacy: dbo.MOrdenServicioResS.Codigo_Est.
/// El valor numérico es el código legacy (mismo criterio que EstadoOrdenServicio):
///   Pendiente = 1 (01), Procesado = 13 (13), Anulado = 4 (04).
/// Lo gobierna la Orden de Servicio: nace Pendiente, pasa a Procesado al procesar la orden y a Anulado
/// al anularla. (El legacy sumaba también el código 09 en los importes, pero nunca lo asignaba.)
/// </summary>
public enum EstadoOrdenServicioSalida
{
    Pendiente = 1,
    Procesado = 13,
    Anulado = 4
}
