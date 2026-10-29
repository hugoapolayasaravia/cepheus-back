using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Maestros.Productos.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Maestros.Productos.GetProductosPaginated
{
    public class GetProductosPaginatedQuery : PagedRequest, IRequest<PagedResult<ProductoResponse>>
    {
        public string? Search { get; set; }
        public string? TipoProductoCode { get; set; }
        public string? CategoryCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
