using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Monedas.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Monedas.GetMonedasPaginated
{
    public class GetMonedasPaginatedQuery : PagedRequest, IRequest<PagedResult<MonedaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
