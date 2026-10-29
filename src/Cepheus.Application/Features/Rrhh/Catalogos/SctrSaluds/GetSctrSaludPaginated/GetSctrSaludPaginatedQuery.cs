using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.SctrSaluds.GetSctrSaludPaginated
{
    public class GetSctrSaludPaginatedQuery : PagedRequest, IRequest<PagedResult<SctrSaludResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}