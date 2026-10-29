namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Resultado de la gestión de una etapa de la Solicitud de Nota de Crédito.
    ///
    /// Legacy: dbo.RCreVentasSol.IND_ESTADO_VTA / _CRED / _CONTABILIDAD / _NEGOCIO / _UC
    /// (char(1)). Los valores legacy no vinieron documentados; SUPUESTO:
    /// Pendiente / Aprobado / Rechazado.
    /// </summary>
    public enum EstadoAprobacionSolicitud
    {
        Pendiente = 0,
        Aprobado = 1,
        Rechazado = 2
    }
}
