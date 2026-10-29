using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposSctr.GetTiposSctrPaginated
{
    public class GetTiposSctrPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoSctrResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}