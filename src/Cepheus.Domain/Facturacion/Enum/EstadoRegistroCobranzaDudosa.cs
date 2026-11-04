namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado del registro de cobranza dudosa (legacy MCobranzaDudosa.Estado): '01' -> Registrado,
    /// 'XX' -> Cancelado (el documento ya fue cobrado por completo; no se puede eliminar el registro).
    /// </summary>
    public enum EstadoRegistroCobranzaDudosa
    {
        Registrado = 1,
        Cancelado = 2
    }
}
