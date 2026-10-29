using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Sexos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Sexos.GetSexosPaginated
{
    public class GetSexosPaginatedQuery : PagedRequest, IRequest<PagedResult<SexoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}