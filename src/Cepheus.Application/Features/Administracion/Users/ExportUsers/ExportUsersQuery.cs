using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Administracion.Users.ExportUsers
{
    /// <summary>
    /// Mismos filtros y orden que el listado (GetUsersPaginatedQuery), pero sin paginar.
    /// Nullable a propósito por el binding de [AsParameters] (ver PagedRequest).
    /// </summary>
    public class ExportUsersQuery : IRequest<ExportFileResponse>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; }
        public bool? SortDesc { get; set; }

        /// <summary>Columnas visibles en pantalla, separadas por coma (ej. "email,firstName,roles").</summary>
        public string? Columns { get; set; }

        /// <summary>Lo fija el endpoint (excel o pdf); el cliente no lo decide.</summary>
        public ExportFormat? Format { get; set; }
    }
}
