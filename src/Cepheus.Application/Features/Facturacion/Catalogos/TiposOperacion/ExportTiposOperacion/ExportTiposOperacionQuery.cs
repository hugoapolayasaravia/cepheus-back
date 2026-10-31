using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.ExportTiposOperacion
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposOperacionQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
