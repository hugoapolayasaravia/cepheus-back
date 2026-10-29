namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Estado de un Documento de Compra. Legacy: dbo.RDocCompras.Estado (char(2)):
    ///   01 Pendiente, 03 Cancelado.
    /// El valor numérico coincide con el código legacy.
    ///
    /// Un documento nace Pendiente. Pasa a Cancelado cuando lo pagado cubre el total
    /// (lo hará el futuro módulo de pagos, que también registra FechaCancelacion).
    /// </summary>
    public enum EstadoDocumentoCompra
    {
        Pendiente = 1,
        Cancelado = 3
    }
}
