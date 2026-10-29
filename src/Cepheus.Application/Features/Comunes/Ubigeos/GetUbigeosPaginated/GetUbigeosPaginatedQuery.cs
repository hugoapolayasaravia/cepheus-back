using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Ubigeos.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Ubigeos.GetUbigeosPaginated
{
    public class GetUbigeosPaginatedQuery : PagedRequest, IRequest<PagedResult<UbigeoResponse>>
    {
        public string? Search { get; set; }
        public string? Department { get; set; }
        public string? Province { get; set; }
        public bool? IsActive { get; set; }
    }
}
