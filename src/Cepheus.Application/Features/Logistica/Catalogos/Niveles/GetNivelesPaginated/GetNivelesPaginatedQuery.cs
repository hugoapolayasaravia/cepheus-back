using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.Niveles.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Niveles.GetNivelesPaginated
{
    public class GetNivelesPaginatedQuery : PagedRequest, IRequest<PagedResult<NivelResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
