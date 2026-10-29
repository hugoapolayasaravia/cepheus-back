using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposArticulo.GetTiposArticuloPaginated
{
    public class GetTiposArticuloPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoArticuloResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}