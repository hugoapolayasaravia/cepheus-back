namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Motor de fórmulas de metrado a aplicar en las líneas de
    /// CotizacionMetradoDetalle (bovedillas/losas aligeradas).
    ///
    /// Legacy: dbo.CotizacionResumen.Flag_Tipo (char(1) '0'/'1'; el DDL
    /// original no lo documentaba, se confirmó vía el código PowerBuilder de
    /// w_cotizaciones: la ventana lo llama "Plantilla a Utilizar" y es
    /// obligatorio solo para el negocio 'PT'). NO CONFUNDIR con
    /// CotizacionResumen.Tipo_Cot (enum TipoCotizacion en este modelo,
    /// Nueva/Recotización/Copia) — son dos columnas legacy distintas que se
    /// habían fusionado por error en una primera versión de este modelo; se
    /// separaron al revisar el código fuente de la ventana original.
    ///
    /// Selecciona qué función SQL usa el cálculo automático de Support
    /// (Apoyo_Cot) al capturar LongitudInt_Cot, y cuáles fórmulas de
    /// Quantity/Row/QuantityP se recalculan en cada edición de línea (ver
    /// ue_actualizavalores vs. ue_actualizavalores1 en el código original):
    ///   V1 (Flag_Tipo <> '1'): dbo.Apoyo_Vigueta_v2(); recalcula Quantity y
    ///                          Row en cada cambio de línea.
    ///   V2 (Flag_Tipo =  '1'): dbo.Apoyo_Vigueta(); Quantity y Row NO se
    ///                          recalculan (quedan al valor que ya tenían).
    /// </summary>
    public enum SistemaCalculoMetrado
    {
        V1 = 1,
        V2 = 2
    }
}
