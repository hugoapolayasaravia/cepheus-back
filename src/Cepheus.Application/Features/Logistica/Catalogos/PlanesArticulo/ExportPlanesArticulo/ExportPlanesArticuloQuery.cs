using Cepheus.Application.Comun.Models;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.ExportPlanesArticulo
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportPlanesArticuloQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
