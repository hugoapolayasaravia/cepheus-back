using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Oportunidades.ExportOportunidades
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportOportunidadesQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
