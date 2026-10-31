using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.ExportTiposSctr
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposSctrQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
