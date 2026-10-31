using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.ExportCentrosEjecutores
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportCentrosEjecutoresQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
