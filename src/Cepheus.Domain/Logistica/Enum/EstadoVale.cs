namespace Cepheus.Domain.Logistica.Enum;

/// <summary>
/// Estado de la cabecera de un Vale de Salida.
/// Legacy: dbo.MValesRes.Codigo_Est. El valor numérico del enum es el código legacy
/// (mismo criterio que EstadoNotaIngreso / EstadoOrdenCompra).
///
///   Pendiente             = 1   (01) registrado, editable
///   Aprobado              = 9   (09) aprobado, listo para preparar / procesar
///   AprobacionProvisional = 30  (30) aprobado por un nivel que no cubre el monto
///                               (reservado; el flujo vigente es el de la OC, ver ChangeEstadoVale)
///   EntregaParcial        = 12  (12) procesado por ítems, quedan líneas pendientes
///   Procesado             = 13  (13) stock descontado
///   Devuelto              = 14  (14) todo el vale fue devuelto al almacén
///   Anulado               = 4   (04)
///
/// El legacy menciona además el 11 (Cerrado) en algunos mensajes, pero ninguna ruta lo asigna.
/// </summary>
public enum EstadoVale
{
    Pendiente = 1,
    Aprobado = 9,
    AprobacionProvisional = 30,
    EntregaParcial = 12,
    Procesado = 13,
    Devuelto = 14,
    Anulado = 4
}
