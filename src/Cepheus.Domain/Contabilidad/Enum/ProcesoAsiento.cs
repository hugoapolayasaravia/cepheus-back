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
        ReversionCajaChica = 7
    }
}
