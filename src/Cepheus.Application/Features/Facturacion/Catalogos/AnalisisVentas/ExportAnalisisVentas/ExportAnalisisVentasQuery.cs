using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.AnalisisVentas.ExportAnalisisVentas
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportAnalisisVentasQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? SegmentoVentasCode { get; set; }
    }
}
