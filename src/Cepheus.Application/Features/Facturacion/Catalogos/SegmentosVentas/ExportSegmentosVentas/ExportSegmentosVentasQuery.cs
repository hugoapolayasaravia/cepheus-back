using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.SegmentosVentas.ExportSegmentosVentas
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportSegmentosVentasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
    }
}
