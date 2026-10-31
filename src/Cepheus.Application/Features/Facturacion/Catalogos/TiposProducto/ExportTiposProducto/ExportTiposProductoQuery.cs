using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.ExportTiposProducto
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportTiposProductoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
