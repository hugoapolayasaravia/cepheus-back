namespace Cepheus.Application.Comun.Models
{
    /// <summary>Archivo listo para descargar.</summary>
    public record ExportFileResponse(byte[] Content, string ContentType, string FileName);
}
