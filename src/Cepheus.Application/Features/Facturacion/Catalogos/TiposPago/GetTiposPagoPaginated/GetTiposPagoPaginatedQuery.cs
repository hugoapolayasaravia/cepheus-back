using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.TiposPago.GetTiposPagoPaginated
{
    public class GetTiposPagoPaginatedQuery : PagedRequest, IRequest<PagedResult<TipoPagoResponse>>
    {
        public string? Search { get; set; }
        public bool? IsActive { get; set; }
    }
}
