namespace Cepheus.Domain.Facturacion.Transacciones
{
    /// <summary>
    /// Archivo de sustento de la solicitud.
    ///
    /// Legacy: dbo.DCreVentasSusArchivos. PK legacy (Codigo_snc, Ano_snc, Mes_snc,
    /// nombre_archivo).
    ///   nombre_archivo -> FileName
    ///   Archivo_Sus (image) -> FilePath: se cambia el binario en BD por la RUTA del
    ///                          archivo (indicado en el script: "Cambiar por ruta").
    /// </summary>
    public class SolicitudNotaCreditoArchivo
    {
        public string Code { get; set; } = default!;
        public string Year { get; set; } = default!;
        public string Month { get; set; } = default!;
        public SolicitudNotaCredito Solicitud { get; set; } = default!;

        public string FileName { get; set; } = default!;
        public string FilePath { get; set; } = default!;
    }
}
