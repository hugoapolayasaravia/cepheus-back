namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Estado de una línea de Vale de Salida. Legacy: dbo.MValesDet.Codigo_Est
/// (01 Pendiente, 13 Procesado, 14 Devolución, 04 Anulado). El valor numérico es el código legacy.
/// </summary>
public enum EstadoValeDetalle
{
    Pendiente = 1,
    Procesado = 13,
    Devuelto = 14,
    Anulado = 4
}
