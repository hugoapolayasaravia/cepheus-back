using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Areas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Areas.GetAreasPaginated
{
    public class GetAreasPaginatedQuery : PagedRequest, IRequest<PagedResult<AreaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}