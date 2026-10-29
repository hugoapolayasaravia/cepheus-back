namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Estado de la cabecera de un Ajuste de Inventario.
/// Legacy: dbo.MAjustesInventario.Codigo_Est. El valor numérico del enum es el código legacy
/// (mismo criterio que EstadoNotaIngreso / EstadoVale).
///
///   Pendiente      = 1   (01) registrado, editable
///   EntregaParcial = 12  (12) procesado por ítems, quedan líneas pendientes
///   Procesado      = 13  (13) stock actualizado
///   Devuelto       = 14  (14) todo lo procesado fue revertido
///   Anulado        = 4   (04)
///
/// El legacy menciona además el 11 (Cerrado) en algunos mensajes, pero ninguna ruta lo asigna.
/// El ajuste no tiene paso de aprobación: se procesa directamente desde Pendiente.
/// </summary>
public enum EstadoAjusteInventario
{
    Pendiente = 1,
    EntregaParcial = 12,
    Procesado = 13,
    Devuelto = 14,
    Anulado = 4
}
