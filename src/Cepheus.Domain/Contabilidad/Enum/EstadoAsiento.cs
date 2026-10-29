namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Estado de un asiento contable. Legacy: dbo.MAsientoResumen.Estado_res (char(2)):
    ///   01 Pendiente, 04 Anulado. El valor numérico coincide con el código legacy.
    /// </summary>
    public enum EstadoAsiento
    {
        Pendiente = 1,
        Anulado = 4
    }
}
