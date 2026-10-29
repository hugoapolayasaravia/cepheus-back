namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Turno de la Hoja de Corte. Legacy: dbo.RHojaCorte.Turno_hco (char(1)):
    ///   'M' -> Manana
    ///   'T' -> Tarde
    ///   'N' -> Noche
    /// Se persiste como ordinal. Enum propio de Facturación, independiente del
    /// Turno de Mantenimiento.
    /// </summary>
    public enum TurnoHojaCorte
    {
        Manana = 0,
        Tarde = 1,
        Noche = 2
    }
}
