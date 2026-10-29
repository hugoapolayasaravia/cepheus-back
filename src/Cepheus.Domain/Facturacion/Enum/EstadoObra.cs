namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la obra. Reemplaza a dbo.TEstados del legacy para el
    /// subconjunto de valores permitido por CK_MObras — mismo criterio que
    /// EstadoCliente. A diferencia de Cliente, Obra admite además el valor
    /// Terminada.
    ///
    /// Mapeo de valores legacy (Estado_obr char(2)):
    ///   05 -> Activo
    ///   06 -> Inactivo
    ///   07 -> Suspendido
    ///   20 -> Terminada
    ///   43 -> PedidoBloqueado
    ///
    /// El legacy no documenta un flujo de transición obligatorio; igual
    /// criterio que EstadoCliente: ChangeObraEstado no restringe transiciones,
    /// salvo que al pasar a Terminada se completan CompletionDate/CompletionUser
    /// automáticamente si no tenían valor.
    /// </summary>
    public enum EstadoObra
    {
        Activo = 5,
        Inactivo = 6,
        Suspendido = 7,
        Terminada = 20,
        PedidoBloqueado = 43
    }
}
