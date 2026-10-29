using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposTransaccion.GetTiposTransaccionPaginated
{
    public class GetTiposTransaccionPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoTransaccionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
