using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.ExportFormasPagoVenta
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportFormasPagoVentaQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public bool? IsCredit { get; set; }
    }
}
