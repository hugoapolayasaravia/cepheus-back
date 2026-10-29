using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposCentroFormacion.GetTiposCentroFormacionPaginated
{
    public class GetTiposCentroFormacionPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoCentroFormacionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}