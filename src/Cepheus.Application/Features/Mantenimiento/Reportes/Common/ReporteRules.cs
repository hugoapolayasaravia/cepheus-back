namespace Cepheus.Application.Features.Mantenimiento.Reportes.Common;

/// <summary>
/// Reglas compartidas por los reportes de Mantenimiento (traducidas de los SP legado de listado de unidades).
/// En los reportes un filtro "T" (todos) del legacy equivale a no enviar el parámetro.
/// </summary>
public static class ReporteRules
{
    /// <summary>Máximo de días del calendario (el legacy llenaba una tabla temporal día a día sin tope).</summary>
    public const int MaxDiasCalendario = 366;

    public static bool EsTodos(string? valor)
        => string.IsNullOrWhiteSpace(valor) || valor.Trim().Equals("T", StringComparison.OrdinalIgnoreCase);

    /// <summary>Código normalizado, o null si el filtro es "todos".</summary>
    public static string? Filtro(string? valor) => EsTodos(valor) ? null : valor!.Trim().ToUpperInvariant();
}
