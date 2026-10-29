using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.MotivosDevolucionArticulo.GetMotivosDevolucionArticuloPaginated
{
    public class GetMotivosDevolucionArticuloPaginatedQuery : PagedRequest, IRequest<PagedResult<MotivoDevolucionArticuloResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
        public bool? AffectsStock { get; set; }
    }
}
