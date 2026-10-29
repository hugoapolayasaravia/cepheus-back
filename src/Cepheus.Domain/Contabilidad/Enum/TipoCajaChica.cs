namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Tipo de caja chica. Legacy: dbo.MCajaChicaResumen.tipo_chi (char(1)):
    ///   F -> FondoFijo, G -> Gastos, R -> Rendir.
    /// </summary>
    public enum TipoCajaChica
    {
        FondoFijo = 1,
        Gastos = 2,
        Rendir = 3
    }
}
