using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposPension.GetTiposPensionPaginated
{
    public class GetTiposPensionPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoPensionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}