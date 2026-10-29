using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposCliente.GetTiposClientePaginated
{
    public class GetTiposClientePaginatedQuery : PagedRequest, IRequest<PagedResult<TipoClienteResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
