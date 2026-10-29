// Cepheus.Domain/Logistica/Enum/EstadoOrdenCompra.cs
namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Estado de una Orden de Compra. Legacy: dbo.MComprasRes.Codigo_Est
    /// (01 Pendiente, 09 Aprobado, 12 Entrega Parcial, 11 Cerrado, 04 Anulado).
    ///
    /// Transiciones (mismo criterio que EstadoPedido — el mecanismo manual
    /// queda disponible mientras el futuro módulo de Nota de Ingreso no
    /// exista, que es quien en la práctica disparará EntregaParcial/Cerrado):
    ///   Pendiente -> Aprobado | Anulado
    ///   Aprobado -> EntregaParcial | Cerrado | Anulado
    ///   EntregaParcial -> Cerrado
    ///   Cerrado -> (final)
    ///   Anulado -> (final)
    ///
    /// La transición a Aprobado valida contra la matriz de aprobación ya
    /// construida (Nivel/TipoTransaccion/RangoAprobacion/AprobadorAsignado,
    /// TipoTransaccion = "OC", confirmado) — ver ChangeEstadoOrdenCompraCommandHandler.
    ///
    /// Toda Orden de Compra nueva nace en Pendiente.
    /// </summary>
    public enum EstadoOrdenCompra
    {
        Pendiente = 1,
        Aprobado = 9,
        EntregaParcial = 12,
        Cerrado = 11,
        Anulado = 4
    }
}