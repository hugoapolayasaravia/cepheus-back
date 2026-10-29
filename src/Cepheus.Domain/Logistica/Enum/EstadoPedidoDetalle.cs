// Cepheus.Domain/Logistica/Enum/EstadoPedidoDetalle.cs
namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Estado de una línea de Pedido (PedidoDetalle). Reemplaza a
    /// dbo.MPedidoDet.Codigo_Est del legacy (3 valores).
    ///
    /// Mapeo de valores legacy:
    ///   01 Pendiente -> Pendiente
    ///   04 Anulado   -> Anulado
    ///   16 En Compra -> EnCompra
    ///   17 CompraParcial  -> cantidad parcialmente tomada por OC
    ///
    /// Toda línea nueva nace en Pendiente.
    /// </summary>
    public enum EstadoPedidoDetalle
    {
        Pendiente = 1,
        Anulado = 4,
        EnCompra = 16,
        CompraParcial = 17
    }
}