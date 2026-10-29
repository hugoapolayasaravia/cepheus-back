using Cepheus.Application.Comun.Models;
using Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.Common;
using MediatR;

namespace Cepheus.Application.Features.Facturacion.Catalogos.FormasPagoVenta.GetFormasPagoVentaPaginated
{
    public class GetFormasPagoVentaPaginatedQuery : PagedRequest, IRequest<PagedResult<FormaPagoVentaResponse>>
    {
        public string? Search { get; set; }
        public bool? IsCredit { get; set; }
        public bool? IsActive { get; set; }
    }
}
