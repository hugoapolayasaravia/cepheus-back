namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Tipo/opción de una nota de cotización (plantilla u ocurrencia).
    ///
    /// Legacy: dbo.CotizacionTNotas.Opcion_not y dbo.CotizacionNotas.Opcion_not
    /// (char(1), catálogo cerrado: 'O' Observación / 'C' Consideraciones /
    /// 'T' Transporte, default 'O') -> enum, mismo criterio que TipoVehiculo:
    /// catálogo cerrado y corto, sin necesidad de altas sin desplegar.
    ///
    /// Se comparte entre NotaCotizacionPlantilla (catálogo reutilizable por
    /// negocio) y CotizacionNota (nota concreta dentro de una cotización).
    /// </summary>
    public enum OpcionNotaCotizacion
    {
        Observacion = 1,
        Consideraciones = 2,
        Transporte = 3
    }
}
