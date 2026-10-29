using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Negocios.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Negocios.GetNegociosPaginated
{
    public class GetNegociosPaginatedQuery : PagedRequest, IRequest<PagedResult<NegocioResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
