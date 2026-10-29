using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.PreciosProducto.GetPreciosProductoPaginated
{
    public class GetPreciosProductoPaginatedQuery : PagedRequest, IRequest<PagedResult<PrecioProductoResponse>>
    {
        public string? FleteCode { get; set; }
        public string? ProductoTipoCode { get; set; }
        public string? ProductoCode { get; set; }
    }
}
