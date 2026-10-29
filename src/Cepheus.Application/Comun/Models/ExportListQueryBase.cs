namespace Cepheus.Application.Comun.Models
{
    /// <summary>
    /// Parámetros comunes de las exportaciones de listados (mismos filtros y orden del
    /// /paged, pero sin paginar). Nullable a propósito por el binding de [AsParameters].
    /// </summary>
    public abstract class ExportListQueryBase
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }

        /// <summary>Columnas visibles en pantalla, separadas por coma (ej. "code,name,isActive").</summary>
        public string? Columns { get; set; }

        /// <summary>Lo fija el endpoint (excel o pdf); el cliente no lo decide.</summary>
        public ExportFormat? Format { get; set; }
    }
}
