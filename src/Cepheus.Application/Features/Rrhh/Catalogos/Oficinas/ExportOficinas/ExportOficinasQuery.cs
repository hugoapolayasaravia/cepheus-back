using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.ExportOficinas
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportOficinasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
