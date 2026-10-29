namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// De dónde sale la cuenta contable de la línea.
    ///   Fija                    : la cuenta indicada en la plantilla (FK a CuentaContable)
    ///   CuentaPorPagarProveedor : la cuenta por pagar del proveedor según la moneda del documento
    ///                             (tabla ProveedorCuentaContable: proveedor + moneda del documento)
    ///   LineaDocumento          : la cuenta de cada línea del documento de compra
    /// </summary>
    public enum OrigenCuentaAsiento
    {
        Fija = 1,
        CuentaPorPagarProveedor = 2,
        LineaDocumento = 3,

        /// <summary>La cuenta contable del subcentro de costo de la línea del vale (solo DetalleVale).</summary>
        SubCentroCosto = 4
    }
}
