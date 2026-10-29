using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposProducto.GetTiposProductoPaginated
{
    public class GetTiposProductoPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoProductoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
