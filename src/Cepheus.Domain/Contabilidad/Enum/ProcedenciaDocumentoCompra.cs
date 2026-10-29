namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Procedencia de la compra que originó el documento.
    /// Legacy: dbo.RDocCompras.Procedencia_com (char(1), nullable):
    ///   'S' -> OrdenServicio, 'C' -> OrdenCompra, 'D' -> CompraDirecta.
    /// Se persiste como int; el mapeo letra -> valor se resuelve en el ETL.
    /// </summary>
    public enum ProcedenciaDocumentoCompra
    {
        OrdenServicio = 1,
        OrdenCompra = 2,
        CompraDirecta = 3
    }
}
