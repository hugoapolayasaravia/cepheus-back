using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Logistica.Catalogos.FormasPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Logistica.Catalogos.FormasPago.GetFormasPagoPaginated
{
    public class GetFormasPagoPaginatedQuery : PagedRequest, IRequest<PagedResult<FormaPagoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsCredit { get; set; }
        public bool? IsActive { get; set; }
    }
}