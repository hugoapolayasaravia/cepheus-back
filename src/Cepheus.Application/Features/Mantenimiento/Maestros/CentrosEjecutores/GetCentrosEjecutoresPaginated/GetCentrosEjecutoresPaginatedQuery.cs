using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.CentrosEjecutores.GetCentrosEjecutoresPaginated
{
    public class GetCentrosEjecutoresPaginatedQuery : PagedRequest, IRequest<PagedResult<CentroEjecutorResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
