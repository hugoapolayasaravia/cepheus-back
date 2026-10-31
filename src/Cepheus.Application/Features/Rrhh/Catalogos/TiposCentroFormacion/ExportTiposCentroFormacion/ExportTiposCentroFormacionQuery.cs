using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.ExportTiposCentroFormacion
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposCentroFormacionQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
