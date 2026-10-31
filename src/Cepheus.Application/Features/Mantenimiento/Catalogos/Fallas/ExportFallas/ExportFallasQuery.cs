using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.ExportFallas
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportFallasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
