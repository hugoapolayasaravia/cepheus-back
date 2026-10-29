using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.Familias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.Familias.GetFamiliasPaginated
{
    public class GetFamiliasPaginatedQuery : PagedRequest, IRequest<PagedResult<FamiliaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}