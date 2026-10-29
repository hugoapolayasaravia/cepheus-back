using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Oficinas.GetOficinasPaginated
{
    public class GetOficinasPaginatedQuery : PagedRequest, IRequest<PagedResult<OficinaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}