using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.Bancos.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.Bancos.GetBancosPaginated
{
    public class GetBancosPaginatedQuery : PagedRequest, IRequest<PagedResult<BancoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
