using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Afps.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Afps.GetAfpsPaginated
{
    public class GetAfpsPaginatedQuery : PagedRequest, IRequest<PagedResult<AfpResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}