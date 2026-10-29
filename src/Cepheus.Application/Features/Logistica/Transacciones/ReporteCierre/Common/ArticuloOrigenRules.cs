namespace Cepheus.Application.Features.Logistica.Transacciones.ReporteCierre.Common;

/// <summary>
/// Filtro Nacional / Importado del reporte de cierre. El artículo no tiene un indicador de origen: el legacy lo
/// deducía del código (Codigo_Art &lt; '6000000' = nacional, &gt; '6000000' = importado) y SOLO para la planta VI;
/// en las demás plantas el filtro se ignoraba. Se replica tal cual, aislado aquí para cambiarlo en un solo lugar
/// cuando exista un indicador real en el artículo.
/// </summary>
public static class ArticuloOrigenRules
{
    public const string Todos = "T";
    public const string Nacional = "N";
    public const string Importado = "I";

    public const string PlantaConClasificacionPorCodigo = "VI";
    public const string CodigoLimite = "6000000";

    public static bool EsValido(string? origen) =>
        origen is Todos or Nacional or Importado;

    /// <summary>True si el filtro realmente se aplica en esa planta.</summary>
    public static bool Aplica(string plantaCode, string origen) =>
        origen != Todos && plantaCode == PlantaConClasificacionPorCodigo;
}
