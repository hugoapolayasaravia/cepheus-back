using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.ExportProductos
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportProductosQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? TipoProductoCode { get; set; }
        public string? CategoryCode { get; set; }
    }
}
