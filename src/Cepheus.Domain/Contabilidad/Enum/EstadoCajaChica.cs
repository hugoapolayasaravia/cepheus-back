namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Estado de la caja chica. Legacy: dbo.MCajaChicaResumen.Estado_chi (char(2)).
    /// El valor numérico coincide con el código legacy:
    ///   01 Pendiente, 03 Cancelado, 04 Anulado, 13 Procesado.
    /// </summary>
    public enum EstadoCajaChica
    {
        Pendiente = 1,
        Cancelado = 3,
        Anulado = 4,
        Procesado = 13
    }
}
