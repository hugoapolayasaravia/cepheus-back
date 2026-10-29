using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesLaborales.GetRegimenesLaboralesPaginated
{
    public class GetRegimenesLaboralesPaginatedQuery : PagedRequest, IRequest<PagedResult<RegimenLaboralResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}