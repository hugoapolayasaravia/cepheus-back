using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.MotivosChequeRebote.GetMotivosChequeRebotePaginated
{
    public class GetMotivosChequeRebotePaginatedQuery : PagedRequest, IRequest<PagedResult<MotivoChequeReboteResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
