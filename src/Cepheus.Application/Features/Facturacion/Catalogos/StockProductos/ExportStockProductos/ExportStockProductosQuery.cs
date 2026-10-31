using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.StockProductos.ExportStockProductos
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportStockProductosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? PlantaCode { get; set; }
        public string? TipoProductoCode { get; set; }
        public string? ProductoCode { get; set; }
    }
}
