using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SituacionesEps.GetSituacionesEpsPaginated
{
    public class GetSituacionesEpsPaginatedQuery : PagedRequest, IRequest<PagedResult<SituacionEpsResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}