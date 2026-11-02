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
        SubCentroCosto = 4,
        /// <summary>La cuenta de la línea del asiento de provisión del documento cancelado (solo PagoDocumento).</summary>
        CuentaProvisionDocumento = 5,
        /// <summary>La cuenta contable de la cuenta bancaria del pago (solo PagoTotal).</summary>
        CuentaBancariaPago = 6,
        /// <summary>
        /// La cuenta de caja o banco donde entra el pago: la de la cuenta bancaria de depósito o la del tipo de pago
        /// por moneda (solo PagoRecibido de CancelacionVenta).
        /// </summary>
        CuentaPago = 7
    }
}
