using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.ExportTiposZona
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposZonaQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
