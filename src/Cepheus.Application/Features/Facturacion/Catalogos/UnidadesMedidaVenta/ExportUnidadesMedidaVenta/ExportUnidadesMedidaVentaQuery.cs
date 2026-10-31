using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.UnidadesMedidaVenta.ExportUnidadesMedidaVenta
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportUnidadesMedidaVentaQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
