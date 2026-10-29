namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Estado de la cabecera de una Orden de Servicio.
/// Legacy: dbo.MOrdenServicioResI.Codigo_Est.
///
/// Mismo criterio que EstadoNotaIngreso: el valor numérico del enum ES el código legacy
/// (se persiste como int), de modo que el mapeo es directo:
///   Pendiente = 1  -> 01
///   Aprobado  = 9  -> 09
///   Procesado = 13 -> 13
///   Cerrado   = 11 -> 11
///   Anulado   = 4  -> 04
///
/// Flujo:
///   Pendiente -> Aprobado  (valida al aprobador contra la matriz de aprobación, TipoTransaccion "OS")
///   Aprobado  -> Procesado (acción "Procesar": actualiza costos de stock y materiales de la OT)
///   Pendiente | Aprobado | Procesado -> Anulado
///   Cerrado y Anulado son estados finales.
///
/// Cerrado queda reservado: el PowerBuilder solo lo valida (bloquea cambios), nunca lo asigna.
/// Los códigos legacy 14 (Devuelta) y 30 (Aprobación provisional) no se migran: solo los generaban
/// flujos de la nota de ingreso / aprobaciones que ahora resuelve la matriz.
/// </summary>
public enum EstadoOrdenServicio
{
    Pendiente = 1,
    Aprobado = 9,
    Procesado = 13,
    Cerrado = 11,
    Anulado = 4
}
