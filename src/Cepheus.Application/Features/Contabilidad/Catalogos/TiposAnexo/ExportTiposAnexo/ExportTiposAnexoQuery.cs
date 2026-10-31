using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Catalogos.TiposAnexo.ExportTiposAnexo
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposAnexoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
