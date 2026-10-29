namespace Cepheus.Application.Comun.Models
{
    /// <summary>
    /// Datos del encabezado y pie estándar de los reportes en PDF.
    /// </summary>
    public record ReporteInfo(
        string Empresa,
        string Titulo,
        string? Subtitulo,
        string GeneradoPor,
        DateTime GeneradoEn);
}
