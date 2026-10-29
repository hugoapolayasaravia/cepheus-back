namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado del cliente. Reemplaza a dbo.TEstados del legacy para el
    /// subconjunto de valores permitido por CK_MClientes — mismo criterio que
    /// EstadoOrdenTrabajo en Mantenimiento.
    ///
    /// Mapeo de valores legacy (Estado_cli char(2)):
    ///   05 -> Activo
    ///   06 -> Inactivo
    ///   07 -> Suspendido
    ///   43 -> PedidoBloqueado
    ///
    /// A diferencia de EstadoOrdenTrabajo, el legacy no documenta un flujo de
    /// transición obligatorio entre estos valores para Cliente, así que
    /// ChangeClienteEstado no restringe transiciones (ver comentario del
    /// handler). Si el negocio exige un flujo específico, avisar para
    /// agregarlo.
    /// </summary>
    public enum EstadoCliente
    {
        Activo = 5,
        Inactivo = 6,
        Suspendido = 7,
        PedidoBloqueado = 43
    }
}
