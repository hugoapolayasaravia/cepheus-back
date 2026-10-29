using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.PlanesArticulo.GetPlanesArticuloPaginated
{
    public class GetPlanesArticuloPaginatedQuery : PagedRequest, IRequest<PagedResult<PlanArticuloResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}