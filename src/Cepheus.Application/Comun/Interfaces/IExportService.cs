using Cepheus.Application.Comun.Models;

namespace Cepheus.Application.Comun.Interfaces
{
    /// <summary>
    /// Genera archivos de exportación a partir de encabezados y filas ya preparadas.
    /// Es genérico: lo usa cualquier pantalla (Usuarios hoy; Roles, Logística, etc. después).
    /// La implementación (ClosedXML y PDFsharp/MigraDoc) vive en Infrastructure.
    /// </summary>
    public interface IExportService
    {
        byte[] ToExcel(string title, IReadOnlyList<string> headers, IReadOnlyList<object?[]> rows);

        /// <summary>PDF horizontal con encabezado y pie estándar (ver ReporteInfo).</summary>
        byte[] ToPdf(ReporteInfo info, IReadOnlyList<string> headers, IReadOnlyList<object?[]> rows);
    }
}
