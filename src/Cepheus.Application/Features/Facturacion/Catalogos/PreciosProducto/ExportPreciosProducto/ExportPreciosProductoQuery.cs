using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.ExportPreciosProducto
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportPreciosProductoQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? FleteCode { get; set; }
        public string? ProductoTipoCode { get; set; }
        public string? ProductoCode { get; set; }
    }
}
