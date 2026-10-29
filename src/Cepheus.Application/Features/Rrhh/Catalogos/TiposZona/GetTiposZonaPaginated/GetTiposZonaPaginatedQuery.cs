using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.TiposZona.GetTiposZonaPaginated
{
    public class GetTiposZonaPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoZonaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}