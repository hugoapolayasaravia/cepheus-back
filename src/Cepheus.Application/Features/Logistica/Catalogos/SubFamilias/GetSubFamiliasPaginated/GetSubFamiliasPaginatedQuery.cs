using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.SubFamilias.GetSubFamiliasPaginated
{
    public class GetSubFamiliasPaginatedQuery : PagedRequest, IRequest<PagedResult<SubFamiliaResponse>>
    {
        public string? Search { get; set; }
        public string? FamiliaCode { get; set; }
        public bool? IsActive { get; set; }
    }
}