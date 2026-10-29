namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Estado del lote de Detracción de Adquiriente. Legacy: dbo.COM_DETRACCION.COD_ESTADO (char(2)):
    ///   01 Pendiente, 19 Emitido, 03 Cancelado.
    /// El valor numérico coincide con el código legacy.
    ///
    /// Flujo: Pendiente (lote generado, editable) -> Emitido (se generó el archivo de depósito masivo)
    /// -> Cancelado (tesorería registró la fecha y el comprobante de pago).
    /// </summary>
    public enum EstadoDetraccion
    {
        Pendiente = 1,
        Emitido = 19,
        Cancelado = 3
    }
}
