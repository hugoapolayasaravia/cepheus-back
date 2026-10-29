using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposPedido.GetTiposPedidoPaginated
{
    public class GetTiposPedidoPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoPedidoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}