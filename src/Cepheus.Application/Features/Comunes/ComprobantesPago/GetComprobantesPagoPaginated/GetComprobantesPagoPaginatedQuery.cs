using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Comunes.ComprobantesPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Comunes.ComprobantesPago.GetComprobantesPagoPaginated
{
    public class GetComprobantesPagoPaginatedQuery : PagedRequest, IRequest<PagedResult<ComprobantePagoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
