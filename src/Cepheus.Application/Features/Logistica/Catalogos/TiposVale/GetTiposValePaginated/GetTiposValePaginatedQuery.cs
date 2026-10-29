using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.TiposVale.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.TiposVale.GetTiposValePaginated
{
    public class GetTiposValePaginatedQuery : PagedRequest, IRequest<PagedResult<TipoValeResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}