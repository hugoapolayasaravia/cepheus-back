namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Tipo de contabilidad a la que pertenece una línea de asiento.
    /// Legacy: dbo.MAsientoDetalle.Tipo_con (char(1), default 'N'):
    ///   'F' -> Financiera, 'N' -> Todos, 'T' -> Tributaria.
    /// Se persiste como int; el mapeo letra -> valor se resuelve en el ETL.
    /// </summary>
    public enum TipoContabilidad
    {
        Financiera = 1,
        Todos = 2,
        Tributaria = 3
    }
}
