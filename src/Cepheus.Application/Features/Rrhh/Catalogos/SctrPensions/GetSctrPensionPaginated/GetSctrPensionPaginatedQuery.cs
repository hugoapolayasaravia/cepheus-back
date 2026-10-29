using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrPensions.GetSctrPensionPaginated
{
    public class GetSctrPensionPaginatedQuery : PagedRequest, IRequest<PagedResult<SctrPensionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}