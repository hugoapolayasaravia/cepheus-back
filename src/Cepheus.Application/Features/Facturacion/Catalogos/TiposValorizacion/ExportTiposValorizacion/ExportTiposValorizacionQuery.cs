using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposValorizacion.ExportTiposValorizacion
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposValorizacionQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
