namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Etapa del flujo de gestión de una Solicitud de Nota de Crédito de Ventas.
    ///
    /// Legacy: dbo.MAE_USER_GESTION_SNC.COD_ETAPA (varchar(2)).
    ///   00 -> Ingresa            (quien registra la solicitud)
    ///   01 -> Ventas
    ///   02 -> CreditosGestiona
    ///   03 -> CreditosAprueba
    ///   04 -> Contabilidad
    ///   05 -> UnidadNegocio
    /// El valor numérico del enum coincide con el código legacy.
    /// </summary>
    public enum EtapaSolicitudNotaCredito
    {
        Ingresa = 0,
        Ventas = 1,
        CreditosGestiona = 2,
        CreditosAprueba = 3,
        Contabilidad = 4,
        UnidadNegocio = 5
    }
}
