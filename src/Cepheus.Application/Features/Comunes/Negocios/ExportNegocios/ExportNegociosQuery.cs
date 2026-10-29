using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Negocios.ExportNegocios
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportNegociosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
