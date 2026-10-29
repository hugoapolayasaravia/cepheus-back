using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Maestros.Transportistas.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Maestros.Transportistas.GetTransportistasPaginated
{
    public class GetTransportistasPaginatedQuery : PagedRequest, IRequest<PagedResult<TransportistaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsOwnFleet { get; set; }
        public bool? IsActive { get; set; }
    }
}
