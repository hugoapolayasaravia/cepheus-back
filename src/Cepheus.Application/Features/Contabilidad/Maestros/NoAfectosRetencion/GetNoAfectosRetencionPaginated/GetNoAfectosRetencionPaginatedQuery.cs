using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.Common;
using MediatR;

namespace Cepheus.Application.Features.Contabilidad.Maestros.NoAfectosRetencion.GetNoAfectosRetencionPaginated
{
    public class GetNoAfectosRetencionPaginatedQuery : PagedRequest, IRequest<PagedResult<NoAfectoRetencionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public string? Type { get; set; }
    }
}
