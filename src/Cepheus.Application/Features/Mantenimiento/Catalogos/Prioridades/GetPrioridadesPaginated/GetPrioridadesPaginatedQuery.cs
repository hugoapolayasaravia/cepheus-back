using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Prioridades.GetPrioridadesPaginated
{
    public class GetPrioridadesPaginatedQuery : PagedRequest, IRequest<PagedResult<PrioridadResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
