using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.RegimenesPensionarios.GetRegimenesPensionariosPaginated
{
    public class GetRegimenesPensionariosPaginatedQuery : PagedRequest, IRequest<PagedResult<RegimenPensionarioResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}