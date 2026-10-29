using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.UnidadesNegocio.ExportUnidadesNegocio
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportUnidadesNegocioQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? ParentCode { get; set; }
    }
}
