using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.Common;
using MediatR;

namespace Cepheus.Application.Features.Mantenimiento.Maestros.SubCentrosEjecutores.GetSubCentrosEjecutoresPaginated
{
    public class GetSubCentrosEjecutoresPaginatedQuery : PagedRequest, IRequest<PagedResult<SubCentroEjecutorResponse>>
    {
        public string? Search { get; set; }
        public string? CentroEjecutorCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
