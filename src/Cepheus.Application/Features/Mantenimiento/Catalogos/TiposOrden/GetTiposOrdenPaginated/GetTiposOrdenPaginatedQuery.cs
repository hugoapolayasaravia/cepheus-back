using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.TiposOrden.GetTiposOrdenPaginated
{
    public class GetTiposOrdenPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoOrdenResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
