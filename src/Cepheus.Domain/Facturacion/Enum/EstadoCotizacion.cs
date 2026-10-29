namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Estado de la cotización.
    ///
    /// Legacy: dbo.CotizacionResumen.Estado_cot (char(2), default '01'
    /// Pendiente). Reemplaza al subconjunto de dbo.TEstados documentado en el
    /// comentario en línea del DDL, mismo criterio que EstadoProyectoCotizacion.
    ///
    /// Mapeo de valores legacy (Estado_cot char(2)):
    ///   01 -> Pendiente (default)
    ///   09 -> Aprobado
    ///   34 -> Aceptado
    ///   38 -> Proyectado
    ///   45 -> NoAceptado
    ///   04 -> Anulado
    /// </summary>
    public enum EstadoCotizacion
    {
        Anulado = 4,
        Pendiente = 1,
        Aprobado = 9,
        Aceptado = 34,
        Proyectado = 38,
        NoAceptado = 45
    }
}
