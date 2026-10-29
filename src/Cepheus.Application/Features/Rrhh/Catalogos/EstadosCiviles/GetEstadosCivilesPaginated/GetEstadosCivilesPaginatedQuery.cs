using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.Common;
using MediatR;

namespace Cepheus.Application.Features.Rrhh.Catalogos.EstadosCiviles.GetEstadosCivilesPaginated
{
    public class GetEstadosCivilesPaginatedQuery : PagedRequest, IRequest<PagedResult<EstadoCivilResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}