namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de cobranza dudosa de un documento de venta (legacy CobranzaDudosa char(1)):
    ///   'N' -> Normal, 'S' -> Dudosa (enviado a cobranza dudosa), 'X' -> Castigada.
    /// Se guarda como int; el mapeo letra -> valor se resuelve en el ETL.
    /// </summary>
    public enum EstadoCobranzaDudosa
    {
        Normal = 0,
        Dudosa = 1,
        Castigada = 2
    }
}
