namespace Cepheus.Application.Comun.Models
{
    /// <summary>
    /// Base para cualquier request paginado/ordenable en cualquier módulo
    /// (Users hoy; Role, Modulo, Logística, etc. después). Cada Query específica
    /// hereda de esta clase y agrega sus propios filtros (ej. Search, IsActive).
    /// </summary>
    public abstract class PagedRequest
    {
        /// <summary>
        /// Nullable a propósito: en el binding de Minimal API ([AsParameters]),
        /// una propiedad "int" no nullable se trata como OBLIGATORIA en la query
        /// string, sin importar el valor por defecto que tenga en C#. El default
        /// real (1, 10, false) se resuelve en el Handler, no acá.
        /// </summary>
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        /// <summary>
        /// Nombre(s) de propiedad por los que ordenar, separados por coma
        /// (ej. "LastName,FirstName"). Se valida cada uno contra las propiedades
        /// reales del tipo en ApplySort — el que no exista se ignora sin romper
        /// el resto. SortDesc aplica a todas las columnas listadas.
        /// </summary>
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }
    }



}
