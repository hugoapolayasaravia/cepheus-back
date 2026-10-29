namespace Cepheus.Domain.Logistica.Enum
{
    /// <summary>
    /// Estado del contribuyente ante SUNAT. Legacy: MProveedores.EstadoSunat
    /// char(1), nullable.
    /// </summary>
    public enum SunatTaxpayerStatus
    {
        Activo,
        BajaDefinitiva,
        BajaProvisional,
        SuspensionTemporal
    }
}
