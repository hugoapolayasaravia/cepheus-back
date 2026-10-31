using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.ExportRegimenesLaborales
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportRegimenesLaboralesQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
