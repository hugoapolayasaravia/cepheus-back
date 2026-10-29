using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SubOcupaciones.GetSubOcupacionesPaginated
{
    public class GetSubOcupacionesPaginatedQuery : PagedRequest, IRequest<PagedResult<SubOcupacionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}