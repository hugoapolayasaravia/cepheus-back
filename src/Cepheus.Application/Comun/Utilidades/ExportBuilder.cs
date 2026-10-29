using Cepheus.Application.Comun.Interfaces;
using Cepheus.Application.Comun.Models;

namespace Cepheus.Application.Comun.Utilidades
{
    /// <summary>Columna exportable: clave (la misma del listado en pantalla), encabezado y valor.</summary>
    public sealed record ExportColumn<T>(string Key, string Header, Func<T, object?> Value);

    /// <summary>
    /// Arma el archivo Excel/PDF de un listado a partir de filas ya cargadas. Lo usan los
    /// handlers de exportación de Roles, Permisos, Módulos, Submódulos y Programas
    /// (Usuarios conserva el suyo).
    /// </summary>
    public static class ExportBuilder
    {
        public const int MaxFilasExcel = 50_000;
        public const int MaxFilasPdf = 5_000;

        public static int MaxFilas(ExportFormat formato)
            => formato == ExportFormat.Pdf ? MaxFilasPdf : MaxFilasExcel;

        public static List<string> Filtros(string? search, bool? isActive, params string?[] extras)
        {
            var filtros = new List<string>();
            if (!string.IsNullOrWhiteSpace(search)) filtros.Add($"Búsqueda: {search.Trim()}");
            if (isActive.HasValue) filtros.Add(isActive.Value ? "Estado: Activos" : "Estado: Inactivos");
            filtros.AddRange(extras.Where(e => !string.IsNullOrWhiteSpace(e))!);
            return filtros;
        }

        public static string Estado(bool isActive) => isActive ? "Activo" : "Inactivo";

        public static string SiNo(bool value) => value ? "Sí" : "No";

        public static ExportFileResponse Build<T>(
            IExportService exportService,
            IReportSettings reportSettings,
            ICurrentUserService currentUser,
            ExportFormat formato,
            string titulo,
            string archivoBase,
            IReadOnlyList<T> items,
            IReadOnlyList<ExportColumn<T>> catalogo,
            string? columnasPedidas,
            string[] columnasPorDefecto,
            IReadOnlyList<string> filtros)
        {
            var maxFilas = MaxFilas(formato);
            if (items.Count > maxFilas)
            {
                throw new InvalidOperationException(
                    $"La exportación supera el máximo de {maxFilas:N0} filas para este formato. Aplica un filtro e inténtalo de nuevo.");
            }

            var porClave = catalogo.ToDictionary(c => c.Key, StringComparer.OrdinalIgnoreCase);

            // Lista cerrada: cualquier clave que no esté en el catálogo se ignora.
            var claves = (columnasPedidas ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(porClave.ContainsKey)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (claves.Count == 0) claves = columnasPorDefecto.ToList();

            var columnas = claves.Select(k => porClave[k]).ToList();
            var encabezados = columnas.Select(c => c.Header).ToList();
            var filas = items
                .Select(item => columnas.Select(c => c.Value(item)).ToArray())
                .ToList();

            var ahora = HoraLocal.Ahora();

            if (formato == ExportFormat.Pdf)
            {
                var subtitulo = (filtros.Count > 0 ? string.Join(" · ", filtros) + " · " : string.Empty)
                    + $"{filas.Count} registro(s)";
                var generadoPor = currentUser.FullName ?? currentUser.Email ?? "—";
                var info = new ReporteInfo(reportSettings.CompanyName, titulo, subtitulo, generadoPor, ahora);
                var pdf = exportService.ToPdf(info, encabezados, filas);
                return new ExportFileResponse(pdf, "application/pdf", $"{archivoBase}_{ahora:yyyyMMdd_HHmm}.pdf");
            }

            var excel = exportService.ToExcel(titulo, encabezados, filas);
            return new ExportFileResponse(
                excel,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{archivoBase}_{ahora:yyyyMMdd_HHmm}.xlsx");
        }
    }
}
