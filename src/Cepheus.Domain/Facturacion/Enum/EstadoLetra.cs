namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la letra de cliente.
    ///
    /// Legacy: dbo.RLetVentas.Estado (char(2)):
    ///   01 -> Pendiente
    ///   02 -> Facturado: letra por cobrar. TODA letra nace así (desde una cancelación tipo 03 o desde Letras Varias)
    ///         y es una deuda del cliente que se paga en otra cancelación (documento tipo 10).
    ///   03 -> Cancelado: letra cobrada por completo.
    ///   04 -> Anulado.
    /// </summary>
    public enum EstadoLetra
    {
        Pendiente = 1,
        Facturado = 2,
        Cancelado = 3,
        Anulado = 4
    }
}
