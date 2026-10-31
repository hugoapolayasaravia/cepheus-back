using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Maquinas.ExportMaquinas
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportMaquinasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
