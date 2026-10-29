namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado del cheque de cliente.
    ///
    /// Legacy: dbo.RCheVentas.Estado (char(2)):
    ///   01 -> Pendiente: cheque diferido aún no liquidado (todavía no se cobra).
    ///   02 -> Facturado: cheque REBOTADO. Vuelve a ser una deuda del cliente (documento tipo 11) y solo
    ///         se cancela en una nueva cancelación ("Solo se Cancelan Cheques Rebotados").
    ///   03 -> Cancelado: cheque cobrado / aplicado.
    ///   04 -> Anulado.
    /// </summary>
    public enum EstadoCheque
    {
        Pendiente = 1,
        Rebotado = 2,
        Cobrado = 3,
        Anulado = 4
    }
}
