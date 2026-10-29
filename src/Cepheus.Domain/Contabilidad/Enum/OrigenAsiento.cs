namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Origen del asiento. Legacy: dbo.MAsientoResumen.origen_flg (char(1), default 'C'):
    ///   'C' -> Contabilidad, 'J' -> CajaChica, 'L' -> Logistica, 'T' -> Transferencia.
    /// Se persiste como int; el mapeo letra -> valor se resuelve en el ETL.
    /// </summary>
    public enum OrigenAsiento
    {
        Contabilidad = 1,
        CajaChica = 2,
        Logistica = 3,
        Transferencia = 4
    }
}
