using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposOperacion.GetTiposOperacionPaginated
{
    public class GetTiposOperacionPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoOperacionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
