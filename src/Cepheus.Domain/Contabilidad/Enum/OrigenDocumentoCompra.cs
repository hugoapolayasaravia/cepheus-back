namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Módulo de origen del Documento de Compra.
    /// Legacy: dbo.RDocCompras.Origen_flg (char(1), default 'C'):
    ///   'C' -> Contabilidad, 'J' -> CajaChica, 'L' -> Logistica, 'T' -> Transportes.
    /// Se persiste como int; el mapeo letra -> valor se resuelve en el ETL.
    /// </summary>
    public enum OrigenDocumentoCompra
    {
        Contabilidad = 1,
        CajaChica = 2,
        Logistica = 3,
        Transportes = 4
    }
}
