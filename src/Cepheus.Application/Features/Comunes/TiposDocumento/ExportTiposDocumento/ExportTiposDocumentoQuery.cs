using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Comunes.TiposDocumento.ExportTiposDocumento
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposDocumentoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
