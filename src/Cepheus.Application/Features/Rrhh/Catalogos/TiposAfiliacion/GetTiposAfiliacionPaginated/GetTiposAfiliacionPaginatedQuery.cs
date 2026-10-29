using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposAfiliacion.GetTiposAfiliacionPaginated
{
    public class GetTiposAfiliacionPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoAfiliacionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}