using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.ListasPrecio.ExportListasPrecio
{
    /// <summary>Exportación a Excel/PDF: mismos filtros y orden que el listado, sin paginar.</summary>
    public class ExportListasPrecioQuery : ExportListQueryBase, IRequest<ExportFileResponse>
    {
        public string? TipoProductoCode { get; set; }
        public string? ProductoCode { get; set; }
        public bool? Vigente { get; set; }
    }
}
