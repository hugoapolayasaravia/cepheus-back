namespace Cepheus.Domain.Facturacion.Enum
{
    /// <summary>
    /// Tipo de cotización.
    ///
    /// Legacy: dbo.CotizacionResumen.Tipo_Cot (char(1), comentario del DDL:
    /// "Cotizacion, Recotizacion, Actualizacion, Copia (puede ser Enum)") -> enum,
    /// catálogo cerrado y corto.
    ///
    /// </summary>
    public enum TipoCotizacion
    {
        Cotizacion = 1,
        Recotizacion = 2,
        Actualizacion = 3,
        Copia = 4
    }
}
