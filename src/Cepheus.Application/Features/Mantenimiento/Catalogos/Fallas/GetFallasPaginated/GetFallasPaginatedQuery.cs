using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Catalogos.Fallas.GetFallasPaginated
{
    public class GetFallasPaginatedQuery : PagedRequest, IRequest<PagedResult<FallaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
