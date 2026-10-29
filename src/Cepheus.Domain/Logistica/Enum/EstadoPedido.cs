// Cepheus.Domain/Logistica/Enum/EstadoPedido.cs  (reemplaza el docstring/valores anteriores)
namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Estado de un Pedido. Legacy: dbo.MPedidoRes.Codigo_Est (9 valores).
    ///
    /// Mapeo: 01 Pendiente, 30 AprobacionProvisional, 09 Aprobado,
    /// 16 EnCompra, 17 CompraParcial, 10 Comprado, 12 EntregaParcial,
    /// 11 Cerrado, 04 Anulado.
    ///
    /// Transiciones válidas (confirmadas por el usuario, ajustadas para que
    /// Comprado y Cerrado sean estados finales sin salida):
    ///   Pendiente -> AprobacionProvisional | Aprobado | Anulado
    ///   AprobacionProvisional -> Aprobado | Anulado   (solo avanza, sin vuelta a Pendiente)
    ///   Aprobado -> EnCompra | Anulado
    ///   EnCompra -> CompraParcial | Comprado | Anulado
    ///   CompraParcial -> Comprado | EntregaParcial | Anulado
    ///   Comprado -> (ninguna, estado final)
    ///   EntregaParcial -> Cerrado
    ///   Cerrado -> (ninguna, estado final)
    ///   Anulado -> (ninguna, estado final)
    ///
    /// Nota: como Comprado ya no tiene salida, el único camino hacia
    /// EntregaParcial/Cerrado es vía CompraParcial. Esto modela dos
    /// desenlaces alternativos de la fase de compra: (a) se compra todo de
    /// una vez y no hay seguimiento de recepción física -> termina en
    /// Comprado; (b) hay recepciones parciales que sí se siguen -> pasa por
    /// CompraParcial -> EntregaParcial -> Cerrado. A partir de EnCompra el
    /// disparador natural será el futuro módulo de Compras/Almacén; el
    /// mecanismo manual (ChangeEstadoPedidoCommand) queda disponible por si
    /// se necesita mientras esos módulos no existen.
    ///
    /// Todo Pedido nuevo nace en Pendiente.
    /// </summary>
    public enum EstadoPedido
    {
        Pendiente = 1,
        Anulado = 4,
        AprobacionProvisional = 30,
        Aprobado = 9,
        EnCompra = 16,
        CompraParcial = 17,
        Comprado = 10,
        EntregaParcial = 12,
        Cerrado = 11
        
    }
}