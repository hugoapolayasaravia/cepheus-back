using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Conductores.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Conductores.GetConductoresPaginated
{
    public class GetConductoresPaginatedQuery : PagedRequest, IRequest<PagedResult<ConductorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
