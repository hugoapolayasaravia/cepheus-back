using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.CategoriasProducto.GetCategoriasProductoPaginated
{
    public class GetCategoriasProductoPaginatedQuery : PagedRequest, IRequest<PagedResult<CategoriaProductoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
