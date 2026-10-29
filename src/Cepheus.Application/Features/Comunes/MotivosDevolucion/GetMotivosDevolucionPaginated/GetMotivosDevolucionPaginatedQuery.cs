using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.MotivosDevolucion.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.MotivosDevolucion.GetMotivosDevolucionPaginated
{
    public class GetMotivosDevolucionPaginatedQuery : PagedRequest, IRequest<PagedResult<MotivoDevolucionResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public bool? AffectsStock { get; set; }
        public bool? EsVenta { get; set; }
    }
}
