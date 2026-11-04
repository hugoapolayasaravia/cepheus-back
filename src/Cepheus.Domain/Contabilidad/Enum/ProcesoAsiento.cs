namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Proceso contable que usa una plantilla de asiento.
    ///   ProvisionCompraNotaIngreso: nota de ingreso por compra -> documento de compra + asiento de provisión
    ///                               (legacy: fc_genasiprocompras, asiento tipo 02).
    ///   TransferenciaNotaIngreso:   nota de ingreso por transferencia entre plantas -> asiento directo
    ///                               (legacy: evento ue_transferencia de la ventana de pase a contabilidad).
    /// </summary>
    public enum ProcesoAsiento
    {
        ProvisionCompraNotaIngreso = 1,
        TransferenciaNotaIngreso = 2,

        /// <summary>Orden de servicio procesada -> documento de compra + asiento de provisión (legacy: w_con_asientosordenservicio).</summary>
        ProvisionOrdenServicio = 3,

        /// <summary>Orden de servicio con documento "00 TRANSFERENCIA..." -> asiento directo (evento ue_transferencia del legacy).</summary>
        TransferenciaOrdenServicio = 4,

        /// <summary>Consumo: un asiento por planta y rango de fechas con las líneas de vales de salida (legacy: w_con_asientosconsumo).</summary>
        ConsumoVales = 5,

        /// <summary>
        /// Provisión de caja chica: por cada comprobante de la caja se genera el asiento de provisión
        /// (legacy: co_asipro_cajachica, asiento tipo 02) y su documento de compra.
        /// </summary>
        ProvisionCajaChica = 6,

        /// <summary>
        /// Reversión / cancelación de caja chica de tipo fondo fijo o por rendir
        /// (legacy: co_asirev_cajachica, asiento tipo 05).
        /// </summary>
        ReversionCajaChica = 7,

        /// <summary>
        /// Provisión de un documento de venta por cobrar: factura (01), boleta (03), nota de crédito (07) o
        /// nota de débito (08) (legacy: co_asipro_ventas, asiento tipo 03). Hoy lo usa la nota de débito.
        /// Debe: cuenta del cliente (TotalDocumento). Haber: cuenta del producto de cada línea (DetalleDocumento)
        /// e IGV (Igv). En nota de crédito las líneas invierten su naturaleza (InvertsOnCreditNote).
        /// </summary>
        ProvisionVenta = 8,
        /// <summary>
        /// Cancelación de documentos de compra con un Pago a Proveedor (legacy: fc_genasicanprv, asiento tipo 01).
        /// Debe: cuenta por pagar del documento (PagoDocumento; Haber en nota de crédito). Haber: retención del
        /// IGV (PagoRetencion) y total a pagar en la cuenta del banco (PagoTotal).
        /// </summary>
        CancelacionProveedor = 9,
        /// <summary>
        /// Cancelación de documentos de venta (cobranza): reversa la cuenta por cobrar de cada documento cancelado y
        /// registra el ingreso a caja o banco, la cobranza dudosa y la diferencia de cambio
        /// (legacy: fc_genasicancelacion, asiento tipo 07). Líneas: DetalleDocumento (cuenta y anexo de la línea de
        /// provisión de cada documento; las notas de crédito van a la naturaleza contraria), PagoRecibido (caja/banco
        /// según el tipo de pago), CobranzaDudosa (una línea al Debe y otra al Haber) y DiferenciaCambioGanancia /
        /// DiferenciaCambioPerdida.
        /// </summary>
        CancelacionVenta = 10,

        /// <summary>
        /// Canje de un documento de venta a cobranza dudosa (legacy: fc_cobranzadudosacanje, asiento tipo
        /// gs_tipocancelacioncli = 07). Reemplaza la provisión del documento por una de cobranza dudosa.
        /// Líneas: DetalleDocumento (cuenta y anexo de la línea 121/123 de la provisión original, al Haber por el saldo),
        /// CobranzaDudosa (tres líneas con cuenta fija: 12910101 Debe, 94110107 Debe y 19210101 Haber en el legacy, todas
        /// por el saldo a tipo de cambio de la cobranza dudosa) y DiferenciaCambioGanancia / DiferenciaCambioPerdida.
        /// </summary>
        CanjeCobranzaDudosa = 11

    }
}
