using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Epss.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Epss.GetEpsPaginated
{
    public class GetEpsPaginatedQuery : PagedRequest, IRequest<PagedResult<EpsResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}