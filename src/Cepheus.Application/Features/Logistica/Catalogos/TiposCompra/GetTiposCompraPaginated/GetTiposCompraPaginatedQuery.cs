using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposCompra.GetTiposCompraPaginated
{
    public class GetTiposCompraPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoCompraResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}