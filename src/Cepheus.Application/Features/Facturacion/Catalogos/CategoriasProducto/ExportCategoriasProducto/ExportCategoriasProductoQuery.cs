using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.ExportCategoriasProducto
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportCategoriasProductoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
