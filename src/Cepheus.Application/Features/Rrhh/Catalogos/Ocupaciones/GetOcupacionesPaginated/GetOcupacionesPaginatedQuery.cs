using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Ocupaciones.GetOcupacionesPaginated
{
    public class GetOcupacionesPaginatedQuery : PagedRequest, IRequest<PagedResult<OcupacionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}