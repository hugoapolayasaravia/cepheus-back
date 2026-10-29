namespace Cepheus.Domain.Contabilidad.Enum
{
    /// <summary>
    /// De dónde sale el anexo de la línea.
    ///   Ninguno        : sin anexo
    ///   Proveedor      : el proveedor del documento, solo si la cuenta de la línea exige tipo de anexo
    ///                    (legacy: "if len(trim(tipo_anexo)) = 3 then anexo = proveedor")
    ///   LineaDocumento : el tipo y código de anexo de cada línea del documento
    /// </summary>
    public enum OrigenAnexoAsiento
    {
        Ninguno = 1,
        Proveedor = 2,
        LineaDocumento = 3,

        /// <summary>
        /// Anexo del enlace contable del artículo en el subcentro de la línea (ArticuloAnexo; si no hay, SubFamiliaAnexo),
        /// solo si la cuenta de la línea exige tipo de anexo (solo DetalleVale).
        /// </summary>
        ArticuloSubCentro = 4
    }
}
