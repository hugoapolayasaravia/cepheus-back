using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Bancos.ExportBancos
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportBancosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
