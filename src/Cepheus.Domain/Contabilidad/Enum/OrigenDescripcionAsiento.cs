namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// Texto con el que se arma la descripción de la línea (máx. 50 caracteres).
    ///   GlosaDocumento  : glosa del documento ("PLANTA Nota de Ingreso CÓDIGO")
    ///   NombreProveedor : razón social del proveedor
    ///   NumeroDocumento : número del documento
    ///   DetalleLinea    : detalle de la línea del documento
    /// </summary>
    public enum OrigenDescripcionAsiento
    {
        GlosaDocumento = 1,
        NombreProveedor = 2,
        NumeroDocumento = 3,
        DetalleLinea = 4,

        /// <summary>Texto fijo de la plantilla (PlantillaAsiento.DescriptionText).</summary>
        TextoPlantilla = 5
    }
}
