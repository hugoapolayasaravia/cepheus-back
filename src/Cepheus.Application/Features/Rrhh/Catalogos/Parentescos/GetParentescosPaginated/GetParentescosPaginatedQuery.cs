using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.Parentescos.GetParentescosPaginated
{
    public class GetParentescosPaginatedQuery : PagedRequest, IRequest<PagedResult<ParentescoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}