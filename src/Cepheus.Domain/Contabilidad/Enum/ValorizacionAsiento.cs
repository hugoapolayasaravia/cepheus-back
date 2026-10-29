namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Valorización de un tipo de asiento.
    ///
    /// Legacy: dbo.TTipAsiento.Tipo_tas (char(1)):
    ///   'H' -> Historico
    ///   'A' -> Ajustado
    ///
    /// Catálogo cerrado de 2 valores -> enum. Se persiste como int (ordinal); el
    /// mapeo letra -> valor se resuelve en el ETL.
    /// </summary>
    public enum ValorizacionAsiento
    {
        Historico = 1,
        Ajustado = 2
    }
}
