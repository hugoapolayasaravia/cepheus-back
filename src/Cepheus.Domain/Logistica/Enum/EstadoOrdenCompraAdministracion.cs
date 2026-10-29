// Cepheus.Domain/Logistica/Enum/EstadoOrdenCompraAdministracion.cs
namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Estado de una Orden de Compra de Administración (recibos por honorarios,
    /// no mueve stock). Legacy: dbo.MCompraR.Codigo_Est
    /// (01 Pendiente, 04 Anulado, 09 Aprobado, 11 Cerrado).
    ///
    /// Enum propio, independiente de EstadoOrdenCompra: son documentos distintos
    /// y esta orden no tiene Entrega Parcial (no maneja artículos ni stock).
    ///
    /// Transiciones:
    ///   Pendiente -> Aprobado | Anulado
    ///   Aprobado  -> Cerrado | Anulado
    ///   Cerrado   -> (final)
    ///   Anulado   -> (final)
    ///
    /// La transición a Aprobado valida contra la matriz de aprobación
    /// (Nivel/TipoTransaccion/RangoAprobacion/AprobadorAsignado), igual que la
    /// Orden de Compra de Logística — ver ChangeEstadoOrdenCompraAdministracionCommandHandler.
    /// </summary>
    public enum EstadoOrdenCompraAdministracion
    {
        Pendiente = 1,
        Anulado = 4,
        Aprobado = 9,
        Cerrado = 11
    }
}
