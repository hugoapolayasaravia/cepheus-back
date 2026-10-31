using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.ExportPrioridades
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportPrioridadesQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
