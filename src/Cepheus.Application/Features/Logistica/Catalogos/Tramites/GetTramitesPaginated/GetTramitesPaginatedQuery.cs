using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.Tramites.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Tramites.GetTramitesPaginated
{
    public class GetTramitesPaginatedQuery : PagedRequest, IRequest<PagedResult<TramiteResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}